// navigating dropdown
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
        if (event.key !== 'ArrowDown' && event.key !== 'ArrowUp') {
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
            6
            focusedElementIndex--;

            if (focusedElementIndex < 0) {
                focusedElementIndex = list.length - 1;
            }

            list[focusedElementIndex].focus();
        }
    });
};


window.clearSearchInput = (inputElement) => {
    if (inputElement) {
        inputElement.value = "";
        inputElement.blur();
    }
}

// blur the input if the active element is not found-product
window.onInputBlur = (DotNetHelper, inputElement) => {
    inputElement.addEventListener('blur', (event) => {
        console.log("Input blurred");
        setTimeout(() => {
            let activeElement = document.activeElement;
            if (activeElement.classList.contains("found-product")) {
                return;
            }
            inputElement.blur();
            /* if (list[i] === document.activeElement) {
                 console.log("Element is active:", list[i]);
                 return;
             }*/
            DotNetHelper.invokeMethodAsync('InputBlur');
        }, 2);
       
    });
}

// if mouse is clicked outside input or dropdown
window.onMouseClick = (DotNetHelper) => {
    document.addEventListener('click', (event) => {
        event.stopPropagation();
        var clickedElement = event.target;
        console.log(clickedElement);
        if (clickedElement.classList.contains("search-input") || clickedElement.classList.contains("found-product") || clickedElement.classList.contains("add-product-button")) {
            return;
        }
        DotNetHelper.invokeMethodAsync('HideSearchDropDown');
    });
}

// if esc is clicked on any part of the document
window.onEscClicked = (DotNetHelper) => {
    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' || event.keyCode === 27) {
            var focusedElement = document.activeElement;
            DotNetHelper.invokeMethodAsync('OnEscClicked');
        }
    });
}
