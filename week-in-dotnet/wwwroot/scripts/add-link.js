'use strict';

function weekInDotNet(d, serviceUrl, baseUrl) {
    var url = d.URL;
    var title = getTitle();
    var author = getAuthor();

    displayForm();

    function getTitle() {
        return find({ tag: 'h1' });
    }

    function getAuthor() {
        return findAny([
            '.author .profile-display-name',
            '.author a',
            { class: 'author' },
            'a[rel~="author"]'
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

        var apiKey = localStorage ? localStorage.getItem('WeekInDotnet.ApiKey') : '';
        var settingsPane = append(form, 'div', {}, {
            position: 'absolute',
            right: margin + 'px',
            backgroundColor: 'transparent',
            textAlign: 'right'
        });
        var settingsButton = append(settingsPane, 'img', {
            src: baseUrl + '/img/gear.svg',
            title: 'Settings',
            alt: 'Settings'
        }, {}, '');
        settingsButton.dataset.credits = 'By Wikimedia Foundation; Santhosh Thottingal ;Alolita SharmaAmir AharoniArun GaneshBrandon HarrisNiklas LaxströmPau GinerSanthosh ThottingalSiebrand Mazeland - Universal Language Selector extension of MediaWiki, https://github.com/wikimedia/mediawiki-extensions-UniversalLanguageSelector/blob/master/resources/images/cog-sprite.svg, Public Domain';
        settingsButton.addEventListener('click', function (e) {
            settingsDropDown.style.display = settingsDropDown.style.display === 'none' ? 'block' : 'none';
        });
        var settingsDropDown = append(settingsPane, 'div', {}, {
            display: 'none',
            backgroundColor: 'white',
            border: 'solid 1px black',
            padding: margin + 'px',
            textAlign: 'left'
        });
        append(settingsDropDown, 'div', {}, {}, 'API Key:');
        appendInput(settingsDropDown, apiKey, 'apikey')
            .addEventListener('blur', function (e) {
                if (localStorage) {
                    localStorage.setItem('WeekInDotnet.ApiKey', e.target.value);
                }
            });

        append(form, 'div', {}, {}, 'Title:');
        appendInput(form, title, 'title');
        append(form, 'div', {}, {}, 'URL:');
        appendInput(form, url, 'url');
        append(form, 'div', {}, {}, 'Author:');
        appendInput(form, author, 'author');
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