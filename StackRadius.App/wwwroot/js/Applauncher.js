document.addEventListener('DOMContentLoaded', function () {
    const appLauncherBtn = document.getElementById('appLauncherBtn');
    appLauncherBtn.addEventListener('click', function () {
        console.log('App launcher button clicked');
        // Your existing code here

        document.addEventListener('DOMContentLoaded', function () {
            const appLauncherBtn = document.getElementById('appLauncherBtn');
            const appLauncherDropdown = document.getElementById('appLauncherDropdown');
            const appItems = document.querySelectorAll('.app-item');
            const menuItems = document.querySelectorAll('.menu-item');

            appLauncherBtn.addEventListener('click', function () {
                appLauncherDropdown.style.display = appLauncherDropdown.style.display === 'none' ? 'block' : 'none';
            });

            appItems.forEach(item => {
                item.addEventListener('click', function () {
                    const targetMenu = this.getAttribute('data-menu-target');
                    menuItems.forEach(menuItem => {
                        menuItem.classList.remove('active');
                        if (menuItem.querySelector(`[data-menu="${targetMenu}"]`)) {
                            menuItem.classList.add('active');
                        }
                    });
                    appLauncherDropdown.style.display = 'none';
                });
            });

            document.addEventListener('click', function (event) {
                if (!appLauncherBtn.contains(event.target) && !appLauncherDropdown.contains(event.target)) {
                    appLauncherDropdown.style.display = 'none';
                }
            });
        });
    });
});
