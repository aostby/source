// Colours (and, when there is one, a faded backdrop) behind a title's details. The images come from this site
// (backdrop/{id}, poster/{id}), so a canvas may read their pixels.
window.kinoArt = (() => {
    function load(url) {
        return new Promise((resolve) => {
            const img = new Image();
            img.decoding = "async";
            img.onload = () => resolve(img.naturalWidth > 0 ? img : null);
            img.onerror = () => resolve(null);
            img.src = url;
        });
    }

    // Two colours: the image's average (without near-black and near-white pixels) and its most saturated tone.
    function colours(img) {
        const size = 48;
        const canvas = document.createElement("canvas");
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext("2d", { willReadFrequently: true });
        ctx.drawImage(img, 0, 0, size, size);
        const data = ctx.getImageData(0, 0, size, size).data;
        let r = 0, g = 0, b = 0, n = 0, best = null, bestScore = -1;
        for (let i = 0; i < data.length; i += 4) {
            const pr = data[i], pg = data[i + 1], pb = data[i + 2];
            const max = Math.max(pr, pg, pb), min = Math.min(pr, pg, pb);
            const light = (max + min) / 2;
            if (light < 18 || light > 238) continue;
            r += pr; g += pg; b += pb; n++;
            const sat = max === 0 ? 0 : (max - min) / max;
            const score = sat * (1 - Math.abs(light - 128) / 128);
            if (score > bestScore) { bestScore = score; best = [pr, pg, pb]; }
        }
        if (n === 0) return null;
        const avg = [Math.round(r / n), Math.round(g / n), Math.round(b / n)];
        return { a: `rgb(${avg.join(",")})`, b: `rgb(${(best ?? avg).join(",")})` };
    }

    async function apply(id, backdropUrl, posterUrl) {
        const el = document.getElementById(id);
        if (!el) return;
        el.classList.remove("has-backdrop", "has-colours");
        el.style.removeProperty("--art-image");

        const backdrop = backdropUrl ? await load(backdropUrl) : null;
        const source = backdrop ?? (posterUrl ? await load(posterUrl) : null);
        if (!source || !document.body.contains(el)) return;

        try {
            const c = colours(source);
            if (c) {
                el.style.setProperty("--art-a", c.a);
                el.style.setProperty("--art-b", c.b);
                el.classList.add("has-colours");
            }
        } catch {
            // A canvas that can't be read (another site's image): no colours, the page looks as before.
        }
        if (backdrop) {
            el.style.setProperty("--art-image", `url("${backdropUrl}")`);
            el.classList.add("has-backdrop");
        }
    }

    return { apply };
})();
