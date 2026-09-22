// ─────────────────────────────────────────────────────────────────────────────
// GRID MODULE — browser side. AG Grid Community 36 (wwwroot/lib/ag-grid, MIT licence).
//
// Two ways to use it:
//
//   1. Automatically — Views/Shared/_Grid.cshtml writes a <div data-sr-grid> and a
//      <script type="application/json" data-sr-grid-config> next to it; this file finds and starts it.
//
//   2. By hand, on any page:
//        SrGrid.list(element, {
//          dataUrl:   '/my/page',        // the server answers GET {dataUrl}/rows (see GridRequest in C#)
//          columns:   [{ field, headerName, dataType, format, width, pinned, isName }, …],
//          pageSize:  25,
//          recordUrl: '/my/page/record?id=', // optional: double-click a row to open it
//          storageKey:'my.page.grid'     // optional: remember this viewer's column layout
//        });
//        SrGrid.table(element, { columns, rows });   // a small read-only grid, rows already loaded
//
// Features (all AG Grid Community): server-side paging with the page-size picker · sorting
// (ctrl-click for several columns) · a filter per column (text / number / date) with an optional
// floating filter row · "Filter across columns…" quick filter · resize, reorder, pin, show/hide,
// autosize columns · row density · checkbox multi-select (with a select-this-page header box) ·
// tooltips · selectable text · CSV export of exactly what is filtered · per-viewer layout memory.
//
// Look: colours, fonts, page sizes, densities and badge colours come from design-tokens.json
// (window.SrDesign, served by /design/tokens.js). Badges use the .sr-status / .sr-rail-chip / .sr-count classes.
//
// The command bar (if the page has one) is wired through data-sr-* hooks:
//   data-sr-refresh  data-sr-filter-toggle  data-sr-export  data-sr-grid-tools  data-sr-quick
//   data-sr-total    data-sr-selection
// ─────────────────────────────────────────────────────────────────────────────
(function () {
  'use strict';
  if (!window.agGrid) { console.warn('SrGrid: AG Grid is not loaded.'); return; }

  var T = window.SrDesign || {};
  var G = T.grid || {};
  var C = T.color || {};
  var F = T.font || {};
  var FMT = T.format || {};
  var DENSITY = G.density || { compact: { rowHeight: 32, headerHeight: 36 }, default: { rowHeight: 42, headerHeight: 44 }, comfortable: { rowHeight: 52, headerHeight: 56 } };
  var icon = window.SrIcon || function () { return ''; };

  // AG Grid's Quartz theme, coloured by the design tokens.
  var theme = agGrid.themeQuartz.withParams({
    fontFamily: F.family, fontSize: F.size, headerFontSize: F.size, headerFontWeight: 600,
    accentColor: C.primary, borderColor: C.border, backgroundColor: C.surface,
    headerBackgroundColor: C.surface, foregroundColor: C.textMuted, headerTextColor: C.text,
    rowHoverColor: C.bg, selectedRowBackgroundColor: C.primaryBg,
    wrapperBorderRadius: T.shape ? T.shape.radius : 8,
  });

  // ── Small helpers ──────────────────────────────────────────────────────────
  function esc(s) {
    return String(s == null ? '' : s).replace(/[&<>"']/g, function (c) {
      return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
    });
  }
  // localStorage, but never fatal (private windows block it).
  function store(key, value) {
    try {
      if (value === undefined) { return JSON.parse(localStorage.getItem(key) || 'null'); }
      if (value === null) { localStorage.removeItem(key); } else { localStorage.setItem(key, JSON.stringify(value)); }
    } catch (e) { /* storage blocked */ }
    return null;
  }
  function url(base, params) {
    var q = Object.keys(params).filter(function (k) { return params[k] != null && params[k] !== ''; })
      .map(function (k) { return k + '=' + encodeURIComponent(params[k]); });
    return base + (q.length ? '?' + q.join('&') : '');
  }
  var TONE = (function () {   // status word → badge colour, from design-tokens.json status.words
    var m = {}, words = (T.status && T.status.words) || {};
    Object.keys(words).forEach(function (tone) { words[tone].forEach(function (w) { m[w.toLowerCase()] = tone; }); });
    return m;
  })();
  function toneOf(word) { return TONE[String(word || '').toLowerCase()] || 'slate'; }
  var EMPTY = '<span class="sr-empty">' + esc(FMT.empty || '—') + '</span>';

  // ── Columns ────────────────────────────────────────────────────────────────
  function formatOf(col) {
    if (col.format) { return col.format; }
    if (col.dataType === 'timestamp') { return 'datetime'; }
    if (col.dataType === 'date') { return 'date'; }
    return 'text';
  }

  // The value as TEXT (what the tooltip shows and what sorting looks at on the client).
  function display(col, row) {
    var v = row[col.field];
    if (col.dataType === 'lookup' && row[col.field + 'Name'] != null) { v = row[col.field + 'Name']; }
    if (v == null || v === '') { return ''; }
    if (typeof v === 'boolean') { return v ? 'Yes' : 'No'; }
    if (Array.isArray(v)) { return v.join(', '); }
    switch (formatOf(col)) {
      case 'currency':
        var c = FMT.currency || {}, n = Number(v);
        return isNaN(n) ? String(v) : (c.symbol || '') + (c.spaceAfterSymbol ? ' ' : '') + n.toLocaleString(c.locale || 'en-IN', { maximumFractionDigits: 2 });
      case 'date': return String(v).slice(0, 10);
      case 'datetime': return String(v).slice(0, 16).replace('T', ' ');
      default:
        if (typeof v === 'number') { return v.toLocaleString('en-IN', { maximumFractionDigits: 4 }); }
        return v;
    }
  }

  // How the value is DRAWN: status badge · rail chip · count · monospace · a link on the name column.
  function renderer(col, recordUrl) {
    var f = formatOf(col);
    return function (p) {
      var t = p.value;
      if (col.isName && recordUrl && p.data && p.data.__id != null) {
        return '<a class="sr-grid__name" href="' + esc(recordUrl + encodeURIComponent(p.data.__id)) + '">' + (t === '' || t == null ? EMPTY : esc(t)) + '</a>';
      }
      if (f === 'count') {
        var n = Number(p.data ? p.data[col.field] : t);
        if (!n && (FMT.count || {}).hideZero !== false) { return EMPTY; }
        return '<span class="sr-count">' + esc((FMT.count || {}).prefix || '') + n + '</span>';
      }
      if (t === '' || t == null) { return EMPTY; }
      switch (f) {
        case 'status': return '<span class="sr-status" data-tone="' + toneOf(t) + '">' + esc(t) + '</span>';
        case 'rail':   return '<span class="sr-rail-chip" data-rail="' + esc(String(t).toUpperCase()) + '">' + esc(t) + '</span>';
        case 'mono':   return '<span class="font-monospace">' + esc(t) + '</span>';
        default:       return esc(t);
      }
    };
  }

  // One AG Grid column definition. The filter follows the data type; ids and lookups are not filtered
  // by value (the quick filter searches lookup NAMES instead).
  function colDef(col, recordUrl, filters, floating) {
    var def = {
      colId: col.field, field: col.field, headerName: col.headerName,
      sortable: col.dataType !== 'textList',
      valueGetter: function (p) { return p.data ? display(col, p.data) : null; },
      tooltipValueGetter: function (p) { return p.value == null || p.value === '' ? null : String(p.value); },
      cellRenderer: renderer(col, recordUrl),
    };
    if (col.width) { def.width = col.width; def.flex = 0; }
    if (col.minWidth) { def.minWidth = col.minWidth; }
    if (col.maxWidth) { def.maxWidth = col.maxWidth; }
    if (col.pinned) { def.pinned = col.pinned; }
    var f = formatOf(col);
    if (f === 'currency' || col.dataType === 'integer' || col.dataType === 'numeric') { def.type = 'rightAligned'; }
    if (f === 'date' || f === 'datetime' || col.dataType === 'uuid') { def.cellClass = 'font-monospace'; }

    def.filter = false;
    if (filters) {
      switch (col.dataType) {
        case 'integer': case 'numeric': def.filter = 'agNumberColumnFilter'; break;
        case 'date': case 'timestamp': def.filter = 'agDateColumnFilter'; break;
        case 'choice': case 'boolean':
          def.filter = 'agTextColumnFilter';
          def.filterParams = { filterOptions: ['equals', 'notEqual', 'blank', 'notBlank'], maxNumConditions: 1 };
          break;
        case 'text':
          def.filter = 'agTextColumnFilter';
          def.filterParams = { filterOptions: ['contains', 'notContains', 'equals', 'notEqual', 'startsWith', 'endsWith', 'blank', 'notBlank'], maxNumConditions: 2 };
          break;
      }
      def.floatingFilter = !!def.filter && floating;
    }
    return def;
  }

  // A header checkbox that selects the rows on THIS page. (AG Grid's own header checkbox does not work
  // with the infinite row model — AG Grid warning #129.)
  function SelectPageHeader() {}
  SelectPageHeader.prototype.init = function (params) {
    var api = params.api, box = document.createElement('input');
    box.type = 'checkbox'; box.className = 'form-check-input m-0'; box.title = 'Select the rows on this page';
    this.eGui = document.createElement('div'); this.eGui.className = 'sr-grid__select-head'; this.eGui.appendChild(box);
    function pageNodes() {
      var size = api.paginationGetPageSize(), first = api.paginationGetCurrentPage() * size, nodes = [];
      api.forEachNode(function (n) { if (n.data && n.rowIndex >= first && n.rowIndex < first + size) { nodes.push(n); } });
      return nodes;
    }
    function sync() {
      var nodes = pageNodes(), picked = nodes.filter(function (n) { return n.isSelected(); }).length;
      box.checked = nodes.length > 0 && picked === nodes.length;
      box.indeterminate = picked > 0 && picked < nodes.length;
    }
    box.addEventListener('change', function () { var on = box.checked; pageNodes().forEach(function (n) { n.setSelected(on); }); });
    this.sync = sync; this.api = api;
    ['selectionChanged', 'paginationChanged', 'modelUpdated'].forEach(function (e) { api.addEventListener(e, sync); });
  };
  SelectPageHeader.prototype.getGui = function () { return this.eGui; };
  SelectPageHeader.prototype.refresh = function () { return true; };
  SelectPageHeader.prototype.destroy = function () {
    var api = this.api, sync = this.sync;
    ['selectionChanged', 'paginationChanged', 'modelUpdated'].forEach(function (e) { api.removeEventListener(e, sync); });
  };

  // ── The Columns ▾ and Density ▾ menus (Bootstrap dropdowns) ─────────────────
  function tools(host, api, cfg, applyDensity) {
    if (!host) { return; }
    host.innerHTML =
      '<div class="dropdown"><button type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside">' + icon('layout-three-columns') + ' Columns</button>' +
      '<div class="dropdown-menu dropdown-menu-end sr-grid__menu" data-cols></div></div>' +
      '<div class="dropdown"><button type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle" data-bs-toggle="dropdown">' + icon('distribute-vertical') + ' Density</button>' +
      '<div class="dropdown-menu dropdown-menu-end" data-density></div></div>';

    var cols = host.querySelector('[data-cols]');
    function drawCols() {
      var state = api.getColumnState();
      cols.innerHTML = '<h6 class="dropdown-header">Columns</h6>' + cfg.columns.map(function (c) {
        var s = state.filter(function (x) { return x.colId === c.field; })[0];
        return '<label class="dropdown-item d-flex gap-2"><input type="checkbox" class="form-check-input" data-col="' + esc(c.field) + '"' + (s && s.hide ? '' : ' checked') + '> ' + esc(c.headerName) + '</label>';
      }).join('') +
        '<div class="dropdown-divider"></div>' +
        '<button type="button" class="dropdown-item" data-autosize>' + icon('arrows') + ' Autosize columns</button>' +
        '<button type="button" class="dropdown-item" data-reset>' + icon('arrow-counterclockwise') + ' Reset layout</button>';
    }
    cols.addEventListener('change', function (e) {
      var f = e.target.getAttribute('data-col');
      if (f) { api.setColumnsVisible([f], e.target.checked); }
    });
    cols.addEventListener('click', function (e) {
      if (e.target.closest('[data-autosize]')) { api.autoSizeAllColumns(); }
      if (e.target.closest('[data-reset]')) { store(cfg.storageKey, null); api.resetColumnState(); api.setFilterModel(null); drawCols(); }
    });
    host.querySelector('[data-bs-toggle]').addEventListener('show.bs.dropdown', drawCols);

    var dens = host.querySelector('[data-density]');
    dens.innerHTML = '<h6 class="dropdown-header">Row density</h6>' + Object.keys(DENSITY).map(function (k) {
      return '<button type="button" class="dropdown-item" data-d="' + k + '">' + k.charAt(0).toUpperCase() + k.slice(1) + '</button>';
    }).join('');
    dens.addEventListener('click', function (e) {
      var b = e.target.closest('[data-d]');
      if (b) { store('sr.grid.density', b.getAttribute('data-d')); applyDensity(b.getAttribute('data-d')); }
    });
  }

  // ── SrGrid.list — a server-paged grid ───────────────────────────────────────
  function list(el, cfg) {
    var page = el.closest('main') || document;   // where to look for the command bar hooks
    var hook = function (name) { return page.querySelector('[data-sr-' + name + ']') || document.querySelector('[data-sr-' + name + ']'); };
    var density = store('sr.grid.density') || cfg.density || G.defaultDensity || 'default';
    var dims = DENSITY[density] || DENSITY['default'];
    var floating = !!store('sr.grid.filterRow');
    var ms = G.multiSelect || {};
    var state = { sortModel: null, filterModel: null, quick: null, tab: null };
    var api;
    var total = hook('total'), selection = hook('selection'), exportLink = hook('export');
    var error = el.parentElement.querySelector('[data-sr-grid-error]');

    function columnDefs() { return cfg.columns.map(function (c) { return colDef(c, cfg.recordUrl, true, floating); }); }

    // The request the server's GridRequest reads — AG Grid's own models, as JSON.
    function params(extra) {
      return Object.assign({
        sortModel: state.sortModel && state.sortModel.length ? JSON.stringify(state.sortModel) : null,
        filterModel: state.filterModel && Object.keys(state.filterModel).length ? JSON.stringify(state.filterModel) : null,
        quick: state.quick, tab: state.tab,
      }, extra || {});
    }
    function syncExport() { if (exportLink) { exportLink.href = url(cfg.dataUrl + '/export', params()); } }

    var options = {
      theme: theme,
      columnDefs: columnDefs(),
      defaultColDef: Object.assign({}, G.defaultColDef, { filter: undefined }),
      rowModelType: 'infinite',          // ask the server for one block at a time
      cacheBlockSize: cfg.pageSize,
      maxBlocksInCache: 20,
      pagination: true,
      paginationPageSize: cfg.pageSize,
      paginationPageSizeSelector: Array.from(new Set([cfg.pageSize].concat(G.pageSizeSelector || [10, 20, 50, 100]))).sort(function (a, b) { return a - b; }),
      rowHeight: dims.rowHeight, headerHeight: dims.headerHeight, floatingFiltersHeight: dims.headerHeight,
      rowSelection: { mode: ms.mode || 'multiRow', checkboxes: ms.checkboxes !== false, headerCheckbox: false, enableClickSelection: !!ms.enableClickSelection },
      selectionColumnDef: { headerComponent: SelectPageHeader, width: G.selectionColumnWidth || 40, minWidth: 40, maxWidth: 40, pinned: 'left', resizable: false, sortable: false, suppressHeaderMenuButton: true, suppressMovable: true, lockPosition: true },
      suppressCellFocus: true, enableCellTextSelection: true, ensureDomOrder: true,
      tooltipShowDelay: 600, animateRows: true, multiSortKey: 'ctrl',
      getRowId: function (p) { return String(p.data.__id); },
      overlayNoRowsTemplate: '<span class="text-body-secondary">' + esc(cfg.emptyText || 'No records.') + '</span>',
      onRowDoubleClicked: function (e) { if (cfg.recordUrl && e.data && e.data.__id != null) { location.href = cfg.recordUrl + encodeURIComponent(e.data.__id); } },
      onSelectionChanged: function (e) {
        if (!selection) { return; }
        var n = e.api.getSelectedRows().length;
        selection.hidden = n === 0; selection.textContent = n + ' selected';
      },
      onSortChanged: function (e) {
        state.sortModel = e.api.getColumnState().filter(function (c) { return c.sort; })
          .sort(function (a, b) { return (a.sortIndex || 0) - (b.sortIndex || 0); })
          .map(function (c) { return { colId: c.colId, sort: c.sort }; });
        syncExport();
      },
      onFilterChanged: function (e) { state.filterModel = e.api.getFilterModel(); syncExport(); },
      // The viewer's own layout: widths, order, pins, hidden columns — remembered per page.
      onColumnResized: function (e) { if (e.finished) { save(); } },
      onColumnMoved: function (e) { if (e.finished) { save(); } },
      onColumnPinned: save, onColumnVisible: save,
      datasource: {
        getRows: function (p) {
          fetch(url(cfg.dataUrl + '/rows', params({ startRow: p.startRow, endRow: p.endRow })), { headers: { Accept: 'application/json' }, credentials: 'same-origin' })
            .then(function (r) { if (!r.ok) { throw new Error(r.status + ' ' + r.statusText); } return r.json(); })
            .then(function (body) {
              if (total) { total.textContent = body.total.toLocaleString('en-IN'); }
              if (error) { error.hidden = true; }
              p.successCallback(body.rows, body.total);
              if (body.total === 0) { api.showNoRowsOverlay(); } else { api.hideOverlay(); }
            })
            .catch(function (err) {
              p.failCallback();
              if (error) { error.hidden = false; error.textContent = 'Could not load rows: ' + err.message; }
            });
        },
      },
    };

    function save() {
      if (!cfg.storageKey || !api) { return; }
      store(cfg.storageKey, api.getColumnState().map(function (c) { return { colId: c.colId, width: c.width, hide: c.hide, pinned: c.pinned, flex: c.flex }; }));
    }

    api = agGrid.createGrid(el, options);
    var saved = cfg.storageKey ? store(cfg.storageKey) : null;
    if (saved) { api.applyColumnState({ state: saved, applyOrder: true }); }

    // Command bar hooks ------------------------------------------------------
    var refresh = hook('refresh');
    if (refresh) { refresh.addEventListener('click', function () { api.purgeInfiniteCache(); loadTabs(); }); }

    var toggle = hook('filter-toggle');
    if (toggle) {
      toggle.setAttribute('aria-pressed', floating ? 'true' : 'false');
      toggle.classList.toggle('active', floating);
      toggle.addEventListener('click', function () {
        floating = !floating;
        store('sr.grid.filterRow', floating || null);
        toggle.setAttribute('aria-pressed', floating ? 'true' : 'false');
        toggle.classList.toggle('active', floating);
        api.setGridOption('columnDefs', columnDefs());
      });
    }

    var quick = hook('quick');
    if (quick) {
      var timer;
      quick.addEventListener('input', function () {
        clearTimeout(timer);
        timer = setTimeout(function () { state.quick = quick.value.trim() || null; syncExport(); api.purgeInfiniteCache(); }, 350);
      });
    }

    tools(hook('grid-tools'), api, cfg, function (d) {
      var dd = DENSITY[d];
      api.setGridOption('rowHeight', dd.rowHeight);
      api.setGridOption('headerHeight', dd.headerHeight);
      api.setGridOption('floatingFiltersHeight', dd.headerHeight);
      api.resetRowHeights();
    });

    // Tabs (work queues): "All" + one per queue; clicking one filters on the server.
    var tabs = cfg.hasTabs ? document.querySelector('[data-sr-tabs="' + el.id + '"]') : null;
    function loadTabs() {
      if (!tabs) { return; }
      fetch(cfg.dataUrl + '/tabs', { credentials: 'same-origin' }).then(function (r) { return r.ok ? r.json() : []; }).then(function (list) {
        var all = 0;
        list.forEach(function (t) {
          all += t.count;
          var c = tabs.querySelector('[data-count="' + t.key + '"]'); if (c) { c.textContent = t.count.toLocaleString('en-IN'); }
          var b = tabs.querySelector('[data-tab="' + t.key + '"] [data-label]'); if (b && t.label) { b.textContent = t.label; }
        });
        var a = tabs.querySelector('[data-count="*"]'); if (a) { a.textContent = all.toLocaleString('en-IN'); }
      });
    }
    if (tabs) {
      loadTabs();
      tabs.addEventListener('click', function (e) {
        var t = e.target.closest('[data-tab]'); if (!t) { return; }
        tabs.querySelectorAll('[data-tab]').forEach(function (x) { x.classList.toggle('active', x === t); });
        state.tab = t.getAttribute('data-tab') || null;
        syncExport();
        api.purgeInfiniteCache();
      });
    }

    syncExport();
    return { api: api, state: state, refresh: function () { api.purgeInfiniteCache(); } };
  }

  // ── SrGrid.table — a small read-only grid with its rows already loaded ──────
  function table(el, cfg) {
    var dims = DENSITY[G.defaultDensity || 'default'] || DENSITY['default'];
    return agGrid.createGrid(el, {
      theme: theme,
      columnDefs: cfg.columns.map(function (c) { return colDef(c, cfg.recordUrl, false, false); }),
      defaultColDef: Object.assign({}, G.defaultColDef, { filter: false }),
      rowData: cfg.rows, domLayout: 'autoHeight',
      rowHeight: dims.rowHeight, headerHeight: dims.headerHeight,
      suppressCellFocus: true, enableCellTextSelection: true, tooltipShowDelay: 600,
    });
  }

  window.SrGrid = { list: list, table: table };

  // Start every grid the partials put on the page.
  document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('script[data-sr-grid-config]').forEach(function (s) {
      var el = document.getElementById(s.getAttribute('data-sr-grid-config'));
      if (el) { list(el, JSON.parse(s.textContent)); }
    });
    document.querySelectorAll('script[data-sr-table]').forEach(function (s) {
      var el = document.getElementById(s.getAttribute('data-sr-table'));
      if (el) { table(el, JSON.parse(s.textContent)); }
    });
  });
})();
