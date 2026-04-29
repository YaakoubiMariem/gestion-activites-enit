(() => {
    const html = document.documentElement;
    const storageKey = "enit-theme";
    const savedTheme = localStorage.getItem(storageKey);

    if (savedTheme) {
        html.setAttribute("data-theme", savedTheme);
    }

    document.querySelector("[data-theme-toggle]")?.addEventListener("click", () => {
        const nextTheme = html.getAttribute("data-theme") === "dark" ? "light" : "dark";
        html.setAttribute("data-theme", nextTheme);
        localStorage.setItem(storageKey, nextTheme);
    });

    document.querySelectorAll(".offcanvas .nav-link-side").forEach(link => {
        link.addEventListener("click", () => {
            const offcanvasElement = document.getElementById("mobileSidebar");
            if (!offcanvasElement || typeof bootstrap === "undefined") return;
            const instance = bootstrap.Offcanvas.getInstance(offcanvasElement);
            instance?.hide();
        });
    });
})();
