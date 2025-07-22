export function bringInToView(elemId) {
    let cardElem = document.getElementById(elemId);

    if(cardElem) {
        cardElem.scrollIntoView({
            behavior: 'smooth',
            block: 'start',
            inline: "nearest"
        });
    }
};