// Enhanced Sidebar Toggle Functionality
(function () {
  "use strict";

  let sidebarCollapsed = false;
  let isMobile = window.innerWidth <= 768;

  // Initialize sidebar functionality when DOM is loaded
  document.addEventListener("DOMContentLoaded", function () {
    initializeSidebar();
    setupEventListeners();
    handleResponsiveChanges();
  });

  function initializeSidebar() {
    const sidebar = document.querySelector(".sidebar");
    const navTexts = document.querySelectorAll(".nav-text");
    const brandText = document.querySelector(".brand-text");
    const navSectionTitles = document.querySelectorAll(".nav-section-title");

    // Set initial state based on screen size
    if (isMobile) {
      sidebar?.classList.add("d-none");
    }

    // Add smooth transitions
    if (sidebar) {
      sidebar.style.transition = "all 0.3s ease";
    }
  }

  function setupEventListeners() {
    // Toggle button functionality
    const toggleButton = document.querySelector(".navbar-toggler");
    if (toggleButton) {
      toggleButton.addEventListener("change", handleSidebarToggle);
    }

    // Handle clicks outside sidebar on mobile
    document.addEventListener("click", function (event) {
      if (isMobile && !sidebarCollapsed) {
        const sidebar = document.querySelector(".sidebar");
        const toggleButton = document.querySelector(".navbar-toggler");

        if (
          sidebar &&
          !sidebar.contains(event.target) &&
          event.target !== toggleButton
        ) {
          closeSidebar();
        }
      }
    });

    // Handle escape key
    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && !sidebarCollapsed && isMobile) {
        closeSidebar();
      }
    });

    // Handle window resize
    window.addEventListener("resize", debounce(handleResponsiveChanges, 250));
  }

  function handleSidebarToggle(event) {
    if (isMobile) {
      toggleMobileSidebar();
    } else {
      // Desktop keeps sidebar fixed
      return;
    }
  }

  function toggleMobileSidebar() {
    const sidebar = document.querySelector(".sidebar");
    const backdrop = getOrCreateBackdrop();

    if (sidebar?.classList.contains("d-none")) {
      // Show sidebar
      sidebar.classList.remove("d-none");
      sidebar.style.transform = "translateX(-100%)";

      // Trigger reflow
      sidebar.offsetHeight;

      sidebar.style.transform = "translateX(0)";
      backdrop.classList.add("show");
      sidebarCollapsed = false;

      // Add animation class
      sidebar.classList.add("slide-in-left");
      setTimeout(() => sidebar.classList.remove("slide-in-left"), 500);
    } else {
      closeSidebar();
    }
  }

  function toggleDesktopSidebar() {
    const sidebar = document.querySelector(".sidebar");
    const content = document.querySelector(".content");
    const navTexts = document.querySelectorAll(".nav-text");
    const brandText = document.querySelector(".brand-text");
    const navSectionTitles = document.querySelectorAll(".nav-section-title");

    if (!sidebar) return;

    sidebarCollapsed = !sidebarCollapsed;

    if (sidebarCollapsed) {
      // Collapse sidebar
      sidebar.classList.add("collapsed");
      sidebar.classList.remove("expanded");

      // Hide text elements
      navTexts.forEach((text) => {
        text.style.opacity = "0";
        setTimeout(() => (text.style.display = "none"), 150);
      });

      if (brandText) {
        brandText.style.opacity = "0";
        setTimeout(() => (brandText.style.display = "none"), 150);
      }

      navSectionTitles.forEach((title) => {
        title.style.opacity = "0";
        setTimeout(() => (title.style.display = "none"), 150);
      });
    } else {
      // Expand sidebar
      sidebar.classList.remove("collapsed");
      sidebar.classList.add("expanded");

      // Show text elements
      setTimeout(() => {
        navTexts.forEach((text) => {
          text.style.display = "inline";
          setTimeout(() => (text.style.opacity = "1"), 50);
        });

        if (brandText) {
          brandText.style.display = "inline";
          setTimeout(() => (brandText.style.opacity = "1"), 50);
        }

        navSectionTitles.forEach((title) => {
          title.style.display = "block";
          setTimeout(() => (title.style.opacity = "1"), 50);
        });
      }, 150);
    }

    // Update content margin with classes
    updateContentMarginOnToggle();
  }

  function closeSidebar() {
    const sidebar = document.querySelector(".sidebar");
    const backdrop = document.querySelector(".sidebar-backdrop");
    const toggleButton = document.querySelector(".navbar-toggler");

    if (sidebar && !sidebar.classList.contains("d-none")) {
      sidebar.style.transform = "translateX(-100%)";

      setTimeout(() => {
        sidebar.classList.add("d-none");
        sidebar.style.transform = "";
      }, 300);

      if (backdrop) {
        backdrop.classList.remove("show");
      }

      if (toggleButton) {
        toggleButton.checked = false;
      }

      sidebarCollapsed = true;
    }
  }

  function getOrCreateBackdrop() {
    let backdrop = document.querySelector(".sidebar-backdrop");

    if (!backdrop) {
      backdrop = document.createElement("div");
      backdrop.className = "sidebar-backdrop";
      backdrop.addEventListener("click", closeSidebar);
      document.body.appendChild(backdrop);
    }

    return backdrop;
  }

  function updateContentMargin() {
    const content = document.querySelector("main");
    if (!content || isMobile) return;

    const sidebarWidth = sidebarCollapsed
      ? "80px"
      : window.innerWidth >= 1200
        ? "300px"
        : "280px";

    content.style.marginLeft = sidebarWidth;
    content.style.transition = "margin-left 0.3s ease";
  }

  function handleResponsiveChanges() {
    const wasMobile = isMobile;
    isMobile = window.innerWidth <= 768;

    const sidebar = document.querySelector(".sidebar");
    const content = document.querySelector("main");

    if (wasMobile !== isMobile) {
      // Reset sidebar state when switching between mobile/desktop
      if (isMobile) {
        // Switch to mobile
        if (sidebar) {
          sidebar.style.width = "";
          sidebar.classList.add("d-none");
          sidebar.classList.remove("collapsed");
        }
        if (content) {
          content.style.marginLeft = "";
        }
        sidebarCollapsed = true;
      } else {
        // Switch to desktop
        if (sidebar) {
          sidebar.classList.remove("d-none");
          sidebar.style.transform = "";
          sidebar.classList.add("expanded");
        }
        sidebarCollapsed = false;
        updateContentMargin();

        // Ensure all text elements are visible on desktop
        const navTexts = document.querySelectorAll(".nav-text");
        const navSectionTitles =
          document.querySelectorAll(".nav-section-title");

        navTexts.forEach((text) => {
          text.style.display = "inline";
          text.style.opacity = "1";
        });

        navSectionTitles.forEach((title) => {
          title.style.display = "block";
          title.style.opacity = "1";
        });
      }
    }
  }

  // Utility function for debouncing
  function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func(...args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }

  // Add smooth scroll behavior for nav links
  document.addEventListener("click", function (event) {
    const navLink = event.target.closest(".nav-link");
    if (navLink && navLink.getAttribute("href")?.startsWith("#")) {
      event.preventDefault();
      const targetId = navLink.getAttribute("href").substring(1);
      const targetElement = document.getElementById(targetId);

      if (targetElement) {
        targetElement.scrollIntoView({
          behavior: "smooth",
          block: "start",
        });
      }
    }
  });

  // Add loading animation for nav items
  function addLoadingAnimations() {
    const navItems = document.querySelectorAll(".nav-item");

    navItems.forEach((item, index) => {
      item.style.opacity = "0";
      item.style.transform = "translateX(-20px)";

      setTimeout(() => {
        item.style.transition = "all 0.3s ease";
        item.style.opacity = "1";
        item.style.transform = "translateX(0)";
      }, index * 100);
    });
  }

  // Initialize loading animations after a short delay
  setTimeout(addLoadingAnimations, 500);

  // Add tooltips functionality
  function addTooltipsToNavItems() {
    const navItems = document.querySelectorAll(".nav-item");
    navItems.forEach((item) => {
      const navText = item.querySelector(".nav-text");
      if (navText) {
        item.setAttribute("data-tooltip", navText.textContent.trim());
      }
    });
  }

  function updateContentMarginOnToggle() {
    const content = document.querySelector("main");
    const sidebar = document.querySelector(".sidebar");

    if (content && sidebar) {
      if (sidebarCollapsed) {
        content.classList.remove("sidebar-expanded");
        sidebar.classList.remove("expanded");
      } else {
        content.classList.add("sidebar-expanded");
        sidebar.classList.add("expanded");
      }
    }
  }

  // Expose global functions for external use
  window.SidebarUtils = {
    close: closeSidebar,
    isCollapsed: () => sidebarCollapsed,
    isMobile: () => isMobile,
  };
})();
