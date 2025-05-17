document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("siteSearchInput");
    const resultBox = document.getElementById("searchResults");

    input.addEventListener("input", function () {
        const term = input.value.toLowerCase().trim();
        resultBox.innerHTML = "";

        clearHighlights();

        if (!term || term.length < 2) return;

        const searchableElements = Array.from(document.querySelectorAll("p, h1, h2, h3, h4, h5, h6, span, small, li, td"))
            .filter(el => el.offsetParent !== null && el.textContent.toLowerCase().includes(term));

        let results = [];
        let idCounter = 0;

        searchableElements.forEach(el => {
            const html = el.innerHTML;
            const regex = new RegExp(`(${term})`, 'gi');

            if (regex.test(html)) {
                const uniqueId = `search-result-${idCounter++}`;
                el.setAttribute("id", uniqueId);
                el.setAttribute("data-search-id", uniqueId);
                el.innerHTML = html.replace(regex, '<span class="highlighted-search-result">$1</span>');

                const snippet = el.textContent.trim().substring(0, 100);
                results.push({ snippet, id: uniqueId });
            }
        });

        results.slice(0, 10).forEach(res => {
            const item = document.createElement("button");
            item.className = "list-group-item list-group-item-action";
            item.textContent = res.snippet;
            item.setAttribute("data-result-id", res.id);


            item.onclick = (e) => {
                e.preventDefault();
                input.blur(); // prevent re-focus issues

                const target = document.getElementById(res.id);
                const firstHighlight = document.querySelector(`#${res.id} .highlighted-search-result`);

                if (firstHighlight) {
                    setTimeout(() => {
                        firstHighlight.scrollIntoView({ behavior: "smooth", block: "center" });
                        firstHighlight.focus({ preventScroll: true });
                        firstHighlight.style.outline = "3px solid #3399ff";
                    }, 50);
                }
            };


            resultBox.appendChild(item);
        });
    });
    input.addEventListener("keydown", function (e) {
        if (e.key === "Enter" && resultBox.firstChild) {
            e.preventDefault();
            const firstResultId = resultBox.firstChild.getAttribute("data-result-id");
            const firstHighlight = document.querySelector(".highlighted-search-result");

            if (firstHighlight) {
                firstHighlight.scrollIntoView({ behavior: "smooth", block: "center" });
                firstHighlight.focus({ preventScroll: true });
                firstHighlight.style.outline = "3px solid #3399ff"; // Optional visual focus
            }
        }
    });


    input.addEventListener("blur", () => {
        setTimeout(() => resultBox.innerHTML = "", 300);
    });

    function clearHighlights() {
        document.querySelectorAll(".highlighted-search-result").forEach(el => {
            const parent = el.parentNode;
            parent.replaceChild(document.createTextNode(el.textContent), el);
            parent.normalize(); // merge adjacent text nodes
            window.scrollTo({ top: 0, behavior: "smooth" });
            2
        });

        document.querySelectorAll("[data-search-id]").forEach(el => {
            el.removeAttribute("data-search-id");
            el.removeAttribute("id");
        });
    }
});
