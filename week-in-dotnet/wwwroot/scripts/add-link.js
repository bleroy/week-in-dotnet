'use strict';

function weekInDotNet() {
    const url = document.URL;
    const title = getTitle(document);
    const author = getAuthor(document);

    alert(url + '\r\n' + title + ' by ' + author);

    function getTitle(d) {
        return find(d, { tag: 'h1' });
    }

    function getAuthor(d) {
        const msdn = findAny(d, [
            '.author .profile-display-name',
            '.author a',
            { class: 'author' }
            ]);
        if (msdn) return msdn;
    }

    function find(d, query) {
        const els =
            (typeof query === 'string') ? [d.querySelector(query)] :
            (query.id) ? [d.getElementById(query.id)] :
            (query.class) ? d.getElementsByClassName(query.class) :
            (query.tag) ? d.getElementsByTagName(query.tag) : null;
        if (els.length == 0 || !els[0]) return null;
        return els[0].innerText.trim();
    }

    function findAny(d, queries) {
        for (const query of queries) {
            const result = find(d, query);
            if (result) return result;
        }
        return null;
    }
}