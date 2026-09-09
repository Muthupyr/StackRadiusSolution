/* ══════════════════════════════════════════════════════════════════════════
 * AsthraUiUx.RazorPages — site.js
 *
 * Zero-dependency vanilla JavaScript. Powers three interactions in the shell:
 *   1. Sidebar collapse / expand      (waffle toggles .sidebar-collapsed)
 *   2. Sidemap area accordion         (click Area → expand/collapse SubAreas)
 *   3. Breadcrumb copy-to-clipboard   (click the copy icon → URL to clipboard)
 *   4. Grid row selection             (click row → visually highlight)
 *
 * No jQuery. No frameworks. Loaded once from _Layout.cshtml with defer.
 * ══════════════════════════════════════════════════════════════════════════ */

(function () {
    'use strict';

    // ─── 1. Sidebar collapse ────────────────────────────────────────────────
    document.addEventListener('click', function (evt) {
        var toggle = evt.target.closest('[data-toggle="sidebar"]');
        if (!toggle) return;
        var shell = document.querySelector('.app-body');
        if (shell) shell.classList.toggle('sidebar-collapsed');
    });

    // ─── 2. Sidemap area accordion ─────────────────────────────────────────
    // Clicking an area header toggles its child SubArea list.
    // Only one Area can be expanded at a time — matches the React app's UX.
    document.addEventListener('click', function (evt) {
        var header = evt.target.closest('.side-map-area-header');
        if (!header) return;
        var currentlyExpanded = header.classList.contains('expanded');

        // Collapse all first.
        document.querySelectorAll('.side-map-area-header.expanded').forEach(function (h) {
            h.classList.remove('expanded');
            var group = h.nextElementSibling;
            if (group) group.style.display = 'none';
        });

        if (!currentlyExpanded) {
            header.classList.add('expanded');
            var group = header.nextElementSibling;
            if (group) group.style.display = '';
        }
    });

    // Auto-expand the Area of the current page on load.
    document.addEventListener('DOMContentLoaded', function () {
        var activeSub = document.querySelector('.side-map-subarea.active');
        if (!activeSub) return;
        var area = activeSub.closest('.side-map-area');
        if (!area) return;
        var header = area.querySelector('.side-map-area-header');
        if (header) {
            header.classList.add('expanded');
            var group = header.nextElementSibling;
            if (group) group.style.display = '';
        }
    });

    // ─── 3. Breadcrumb copy-to-clipboard ───────────────────────────────────
    document.addEventListener('click', function (evt) {
        var btn = evt.target.closest('.breadcrumb-copy');
        if (!btn) return;
        var url = window.location.href;
        try {
            navigator.clipboard.writeText(url);
            btn.dataset.copied = '1';
            setTimeout(function () { btn.dataset.copied = ''; }, 900);
        } catch (e) { /* clipboard unavailable — silently ignore */ }
    });

    // ─── 4. Grid row selection ─────────────────────────────────────────────
    document.addEventListener('click', function (evt) {
        var row = evt.target.closest('.grid-table tbody tr');
        if (!row) return;
        var wasSelected = row.classList.contains('selected');
        row.parentElement.querySelectorAll('tr.selected').forEach(function (r) { r.classList.remove('selected'); });
        if (!wasSelected) row.classList.add('selected');
    });
})();
