window.focusElementOnKeyPress = () => {
    document.addEventListener('keydown', (event) => {
        const inputElement = document.getElementById("autocomplete-input");
        if (inputElement && !inputElement.matches(':focus')) {
            inputElement.focus();
        }
    });
};