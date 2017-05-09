'use strict';

function weekInDotNet(d, apiKey, serviceUrl, baseUrl) {
    var url = d.URL;
    var title = getTitle();
    var author = getAuthor();

    displayForm();

    function getTitle() {
        return find({ tag: 'h1' });
    }

    function getAuthor() {
        return findAny([
            '.author-name',
            '.author .profile-display-name',
            '.author a',
            { class: 'author' },
            'a[rel~="author"]',
            '.footer-text a[href="/about"]',
            '.copyright'
            ]);
    }

    function find(query) {
        var els =
            (typeof query === 'string') ? [d.querySelector(query)] :
            (query.id) ? [d.getElementById(query.id)] :
            (query.class) ? d.getElementsByClassName(query.class) :
            (query.tag) ? d.getElementsByTagName(query.tag) : null;
        if (els.length == 0 || !els[0]) return '';
        return els[0].innerText.trim();
    }

    function findAny(queries) {
        for (var query of queries) {
            var result = find(query);
            if (result) return result;
        }
        return '';
    }

    function displayForm() {
        var formWidth = 500;
        var margin = 8;

        var dialog = create('div', {}, {
            border: '1px solid black',
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            width: formWidth + 'px',
            margin: '2px auto',
            backgroundColor: 'white',
            zIndex: 999999,
            fontSize: 'small',
            fontFamily: 'sans-serif'
        });
        var titleBar = append(dialog, 'div', {}, {
            left: 0,
            right: 0,
            backgroundColor: 'black',
            color: 'white',
            fontWeight: 'bold',
            padding: margin + 'px'
        }, 'Add this post to The Week in .NET');

        append(titleBar, 'button', {}, {
            float: 'right',
            border: 'none',
            background: 'transparent'
        }, 'X')
            .addEventListener('click', function () {
                d.body.removeChild(dialog);
            });

        var form = append(dialog, 'form', {
            method: 'POST',
            action: serviceUrl
        }, {
            width: formWidth + 'px',
            padding: margin + 'px'
        });

        append(form, 'input', {type: 'hidden', name: 'apikey', value: apiKey})
        append(form, 'div', {}, {}, 'Title:');
        appendInput(form, title, 'title');
        append(form, 'div', {}, {}, 'URL:');
        appendInput(form, url, 'url');
        append(form, 'div', {}, {}, 'Author:');
        appendInput(form, author, 'author');
        // TODO: replace this with dynamically obtained categories
        var categories = append(form, 'fieldset', {}, {});
        append(categories, 'legend', {}, {}, 'Category:');
        append(categories, 'input', { id: 'widn-category-dotnet', type: 'radio', name: 'category', value: '.NET', checked: 'checked' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-dotnet' }, { marginRight: '8px' }, '.NET');
        append(categories, 'input', { id: 'widn-category-aspnet', type: 'radio', name: 'category', value: 'ASP.NET' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-aspnet' }, { marginRight: '8px' }, 'ASP.NET');
        append(categories, 'input', { id: 'widn-category-csharp', type: 'radio', name: 'category', value: 'C#' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-csharp' }, { marginRight: '8px' }, 'C#');
        append(categories, 'input', { id: 'widn-category-fsharp', type: 'radio', name: 'category', value: 'F#' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-fsharp' }, { marginRight: '8px' }, 'F#');
        append(categories, 'input', { id: 'widn-category-vb', type: 'radio', name: 'category', value: 'VB' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-vb' }, { marginRight: '8px' }, 'VB');
        append(categories, 'input', { id: 'widn-category-xamarin', type: 'radio', name: 'category', value: 'Xamarin' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-xamarin' }, { marginRight: '8px' }, 'Xamarin');
        append(categories, 'input', { id: 'widn-category-azure', type: 'radio', name: 'category', value: 'Azure' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-azure' }, { marginRight: '8px' }, 'Azure');
        append(categories, 'input', { id: 'widn-category-uwp', type: 'radio', name: 'category', value: 'UWP' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-uwp' }, { marginRight: '8px' }, 'UWP');
        append(categories, 'input', { id: 'widn-category-data', type: 'radio', name: 'category', value: 'Data' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-data' }, { marginRight: '8px' }, 'Data');
        append(categories, 'input', { id: 'widn-category-game', type: 'radio', name: 'category', value: 'Game development' }, { marginRight: '4px' });
        append(categories, 'label', { for: 'widn-category-game' }, { marginRight: '8px' }, 'Game development');
        append(form, 'button', {
            type: 'submit'
        }, {
            marginTop: '8px'
        }, 'Submit');
        d.body.insertBefore(dialog, d.body.firstChild);

        function create(tag, properties, css, contents) {
            var el = d.createElement(tag || 'div');
            if (properties) {
                for (var prop in properties) {
                    el[prop] = properties[prop];
                }
            }
            if (css) {
                for (var prop in css) {
                    el.style[prop] = css[prop];
                }
            }
            if (contents) el.innerHTML = contents;
            return el;
        }

        function append(parent, tag, properties, css, contents) {
            var el = create(tag, properties, css, contents);
            parent.appendChild(el);
            return el;
        }

        function appendInput(parent, val, name) {
            return append(parent, 'input',
                { value: val, name: name, type: 'text' },
                {
                    width: (formWidth - 5 * margin) + 'px',
                    marginBottom: '8px'
                }
            );
        }
    }
}