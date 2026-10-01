(function () {
  var body = document.body;

  if (document.getElementById("wrapper")) {
    if (window.localStorage.getItem("collapse_menu") === "on") {
      body.classList.add("mini-navbar");
    }

    var syncBodySmall = function () {
      body.classList.toggle("body-small", window.innerWidth < 769);
    };

    syncBodySmall();
    window.addEventListener("resize", syncBodySmall);

    document.querySelectorAll(".navbar-minimalize").forEach(function (button) {
      button.addEventListener("click", function (event) {
        event.preventDefault();
        var collapsed = body.classList.toggle("mini-navbar");
        window.localStorage.setItem("collapse_menu", collapsed ? "on" : "off");
      });
    });
  }

  var profile = document.querySelector("[data-profile]");
  if (profile) {
    var toggle = profile.querySelector("[data-profile-toggle]");
    var menu = profile.querySelector(".dropdown-menu");
    var setOpen = function (open) {
      profile.classList.toggle("show", open);
      if (menu) {
        menu.classList.toggle("show", open);
      }
    };

    if (toggle) {
      toggle.addEventListener("click", function (event) {
        event.preventDefault();
        setOpen(!profile.classList.contains("show"));
      });
    }

    document.addEventListener("click", function (event) {
      if (!profile.contains(event.target)) {
        setOpen(false);
      }
    });
  }

  document.querySelectorAll("[data-logout]").forEach(function (link) {
    link.addEventListener("click", function (event) {
      event.preventDefault();
      var form = document.getElementById("logout-form");
      if (form) {
        form.submit();
      }
    });
  });

  document.querySelectorAll("[data-ibox]").forEach(function (box) {
    var collapse = box.querySelector(".collapse-link");
    var close = box.querySelector(".close-link");
    var content = box.querySelector(".ibox-content");
    var icon = collapse ? collapse.querySelector("i") : null;

    if (collapse && content) {
      collapse.addEventListener("click", function (event) {
        event.preventDefault();
        var collapsed = content.hasAttribute("hidden");
        if (collapsed) {
          content.removeAttribute("hidden");
          box.classList.remove("border-bottom");
          if (icon) {
            icon.classList.add("fa-chevron-up");
            icon.classList.remove("fa-chevron-down");
          }
        } else {
          content.setAttribute("hidden", "");
          box.classList.add("border-bottom");
          if (icon) {
            icon.classList.remove("fa-chevron-up");
            icon.classList.add("fa-chevron-down");
          }
        }
      });
    }

    if (close) {
      close.addEventListener("click", function (event) {
        event.preventDefault();
        box.remove();
      });
    }
  });

  document.querySelectorAll("[data-icheck]").forEach(function (box) {
    var input = box.querySelector('input[type="checkbox"]');
    if (!input) {
      return;
    }

    var sync = function () {
      box.classList.toggle("checked", input.checked);
    };

    sync();
    input.addEventListener("change", sync);
    box.addEventListener("mouseenter", function () {
      box.classList.add("hover");
    });
    box.addEventListener("mouseleave", function () {
      box.classList.remove("hover");
    });
  });

  var loginForm = document.getElementById("login-form");
  if (loginForm) {
    loginForm.addEventListener("submit", function () {
      if (!loginForm.checkValidity()) {
        return;
      }

      var button = loginForm.querySelector('button[type="submit"]');
      if (button) {
        button.disabled = true;
        button.textContent = "Ingresando…";
      }
    });
  }
})();
