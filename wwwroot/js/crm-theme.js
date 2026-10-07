(function () {

    const savedTheme = localStorage.getItem("acxiom-theme") || "emerald";
    document.documentElement.setAttribute("data-theme", savedTheme);

    window.setCrmTheme = function(theme) {
        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem("acxiom-theme", theme);

        document.querySelectorAll(".crm-theme-option").forEach(x => {
            x.classList.toggle("active", x.dataset.theme === theme);
        });
    };

    window.toggleThemePanel = function () {
        const panel = document.getElementById("crmThemePanel");
        if (panel) panel.classList.toggle("show");
    };

    window.toggleUserMenu = function () {
        const menu = document.getElementById("crmUserMenu");
        if (menu) menu.classList.toggle("show");
    };

    document.addEventListener("click", function (event) {

        const user = document.querySelector(".crm-top-user");
        const menu = document.getElementById("crmUserMenu");

        if (menu && user && !user.contains(event.target)) {
            menu.classList.remove("show");
        }

        const themePanel = document.getElementById("crmThemePanel");
        const themeButton = document.getElementById("crmThemeButton");

        if (
            themePanel &&
            themeButton &&
            !themePanel.contains(event.target) &&
            !themeButton.contains(event.target)
        ) {
            themePanel.classList.remove("show");
        }
    });

    document.addEventListener("DOMContentLoaded", function () {
        setCrmTheme(savedTheme);
    });

})();
