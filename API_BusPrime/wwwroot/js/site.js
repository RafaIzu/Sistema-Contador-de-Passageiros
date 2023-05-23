
$(window).on("load", function (event) {
    const showNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId)

        // Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            $(toggle).on('click', () => {
                nav.classList.toggle('show')
                toggle.classList.toggle('bi-x')
                bodypd.classList.toggle('body-pd')
                headerpd.classList.toggle('body-pd')
            })
        }
    }
    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

    const linkColor = document.querySelectorAll('.nav_link')
    function colorLink() {
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'))
            this.classList.add('active')
        }
    }
    linkColor.forEach(l => l.addEventListener('click', colorLink))

    // Your code to run since DOM is loaded and ready

    // Filter table 

});

function filterTable(event, target, url) {
    if (event.keyCode == 13) {
        window.location = url += $(target).val();
    }
    console.log(event, target, url)

}

function excluir(el, url) {
    var modal = $(`${$(el).data('target')}`);
    $(modal).modal('show')
    $(modal).find().unbind('submit');
    $(modal).find('form').attr('action', url);
    $(modal).find('form').on('submit', function (e) {
        $(modal).find('.btn-delete').attr('disabled', true);
    });
}

function getTakeCount() {
    var value = $('#take').val();
    return value;
}

function getCurrentPage() {
    var value = $('.page-item.active').data('page');
    return value;
}

function setListUrl(page) {
    var query = '';
    var take = getTakeCount();
    page = page ?? getCurrentPage();
    console.log(take, page)
    if (take) {
        query += `?take=${take}`;
    }
    if (take && page) {
        query += `&page=${page}`;
    } else if (page) {
        query += `?page=${page}`;
    }
    return query;
}