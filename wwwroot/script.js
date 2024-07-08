window.focusElementOnKeyPress = (inputElement, inputDropdown) => {
    //get ul li list of elements from child elements of inputdropdown
    let focusedElementIndex = 0;
    const list = inputDropdown.children[0].children;

    inputElement.addEventListener('keydown', (event) => {
        if (event.key === 'ArrowDown' && event.key === 'ArrowUp') {
            inputElement.blur();

        }
    });

    document.addEventListener('keydown', (event) => {

        // I want to check event key agains all the letters except arrows up and down
        if (event.key !== 'ArrowDown' && event.key !== 'ArrowUp'){
            inputElement.focus();
        }

        if (event.key === 'ArrowDown') {

            const focusedElement = document.activeElement;

            if (focusedElement.tagName !== 'LI') {
                console.log(list[0]);
                list[0].focus();
                focusedElementIndex = 0;
                return;
            }

            focusedElementIndex++;

            if (focusedElementIndex >= list.length) {
                focusedElementIndex = 0;
            }

            list[focusedElementIndex].focus();
        }

        if (event.key === 'ArrowUp') {
            const focusedElement = document.activeElement;

            console.log(list);
            console.log(focusedElement);
            //if focused element is of the html type li focus on the first element of the list variable
            if (focusedElement.tagName !== 'LI') {
                console.log(list[list.length - 1]);
                list[0].focus();
                focusedElementIndex = 0;
                return;
            }

            focusedElementIndex--;

            if (focusedElementIndex < 0) {
                focusedElementIndex = list.length - 1;
            }

            list[focusedElementIndex].focus();
        }


        

        console.log(event.key);

    });



};


window.clearSearchInput = (inputElement) => {
    if (inputElement) {
        inputElement.value = "";
        inputElement.blur();
    }
}