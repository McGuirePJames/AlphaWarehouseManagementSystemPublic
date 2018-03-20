$(document).ready(function (e) {
    colorElement(getCurrentNavigationElement());

});

function getCurrentNavigationElement() {
    let navigationElements = document.getElementsByClassName('navigation-item');

    let windowController = window.location.pathname.split('/')[1];
    let windowAction = window.location.pathname.split('/')[2].replace('/','')

    for (let i = 0; i < navigationElements.length; i++) {

        let elementController = navigationElements[i].dataset.location.split('/')[0];
        let elementAction = navigationElements[i].dataset.location.split('/')[1]

        if (elementController.toUpperCase() === windowController.toUpperCase() && elementAction.toUpperCase() === windowAction.toUpperCase()) {
            return navigationElements[i]
        }
    }
    return;
}
function colorElement(ele) {
    ele.setAttribute('style', 'background-color: #0073AC')
}