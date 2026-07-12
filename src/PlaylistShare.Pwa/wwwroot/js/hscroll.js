// Горизонтальные скролл-полосы поиска (карусели "Исполнители/Альбомы/Плейлисты", чипы):
// вертикальное колесо мыши крутит их по горизонтали. Тач и трекпад работают нативно
// (overflow-x: auto + touch-action: auto) - здесь добавляется только поддержка обычного
// вертикального колеса на десктопе, где иначе горизонтальную полосу прокрутить нечем.
//
// Делегированный слушатель на document покрывает и уже отрисованные полосы, и будущие
// (результаты поиска перерисовываются) - переподключать ничего не нужно.
(function () {
    'use strict';
    var SELECTOR = '.deka-search-hscroll, .deka-search-chips';

    document.addEventListener('wheel', function (e) {
        var el = e.target && e.target.closest ? e.target.closest(SELECTOR) : null;
        if (!el) return;
        if (el.scrollWidth <= el.clientWidth) return;          // нечего скроллить
        if (Math.abs(e.deltaX) > Math.abs(e.deltaY)) return;   // трекпад уже даёт горизонталь - не мешаем

        var atStart = el.scrollLeft <= 0;
        var atEnd = Math.ceil(el.scrollLeft + el.clientWidth) >= el.scrollWidth;
        // На краях отдаём прокрутку странице, чтобы колесо не "залипало" на карусели.
        if ((e.deltaY < 0 && atStart) || (e.deltaY > 0 && atEnd)) return;

        el.scrollLeft += e.deltaY;
        e.preventDefault();
    }, { passive: false });
})();
