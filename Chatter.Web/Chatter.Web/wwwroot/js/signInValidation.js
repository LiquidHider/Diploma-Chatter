const loginField = document.getElementById("loginField");

const passwordField = document.getElementById("passwordField");

const loginFieldErrorMessage = document.getElementById("loginFieldErrorMessage");

const passwordFieldErrorMessage = document.getElementById("passwordFieldErrorMessage");


const validators = [
    {
        field: loginField,
        validate: () => {
            const login = loginField.value.trim();
            const errors = [];

           if (login.length == 0) {
                errors.push("Login field is empty.");
           }

            loginFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
    {
        field: passwordField,
        validate: () => {
            const password = passwordField.value.trim();
            const errors = [];

            if (password.length == 0) {
                errors.push("Password field is empty.");
            }

            passwordFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    }
];

validators.forEach(({ field, validate }) => {
    field.addEventListener("input", () => {
        validators.every(({ validate }) => validate());
    });
});
