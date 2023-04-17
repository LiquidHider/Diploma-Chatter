const submitButton = document.getElementById("registerButton");
submitButton.disabled = true;

const emailField = document.getElementById("emailField");
const emailFieldErrorMessage = document.getElementById("emailFieldError");

const userTagField = document.getElementById("userTagField");
const userTagFieldErrorMessage = document.getElementById("userTagFieldError");

const lastNameField = document.getElementById("lastNameField");
const lastNameFieldErrorMessage = document.getElementById("lastNameFieldError");


const firstNameField = document.getElementById("firstNameField");
const firstNameFieldErrorMessage = document.getElementById("firstNameFieldError");


const patronymicField = document.getElementById("patronymicField");
const patronymicFieldErrorMessage = document.getElementById("patronymicFieldError");

const universityNameField = document.getElementById("universityNameField");
const universityNameFieldErrorMessage = document.getElementById("universityNameFieldError");

const universityFacultyField = document.getElementById("universityFacultyField");
const universityFacultyFieldErrorMessage = document.getElementById("universityFacultyFieldError");

const passwordField = document.getElementById("passwordField");
const passwordFieldErrorMessage = document.getElementById("passwordFieldError");

const confirmPasswordField = document.getElementById("confirmPasswordField");
const confirmPasswordFieldErrorMessage = document.getElementById("confirmPasswordFieldError");


const errorMessages = {
    email: {
        required: "Поле електронної адреси обов'язкове для заповнення",
        invalid: "Введена електронна адреса не є дійсною"
    },
    userTag: {
        invalid: "Неприпустимий користувацький тег",
    }
};

const validators = [
    {
        field: userTagField,
        validate: () => {
            const userTag = userTagField.value.trim();
            const errors = [];
            if (userTag.length > 0) {
                if (userTag.length < 2 || userTag.length > 20) {
                    errors.push(errorMessages.userTag.invalid);
                }
            }

            userTagFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
    {
        field: emailField,
        validate: () => {
            const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
            const email = emailField.value.trim();
            const errors = [];
         
            if (email === "" ) {
                errors.push(errorMessages.email.required);
            }

            if (!regex.test(email) && errors.length == 0) {
                errors.push(errorMessages.email.invalid);
            }

            emailFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";


            return errors.length === 0;
        },
    },
    {
        field: lastNameField,
        validate: () => {
            const lastName = lastNameField.value.trim();
            const errors = [];

            if (lastName === "") {
                errors.push("Поле 'Прізвище' є обов'язковим для заповнення");
            }

            if (lastName.length < 2 || lastName.length > 20) {
                errors.push("Довжина 'Прізвище' повинна бути від 2 до 20 символів");
            }

            if (/\d/.test(lastName)) {
                errors.push("Поле 'Прізвище' не повинно містити цифри");
            }

            lastNameFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
    {
        field: firstNameField,
        validate: () => {
            const firstName = firstNameField.value.trim();
            const errors = [];

            if (firstName === "") {
                errors.push("Поле 'Ім'я' є обов'язковим для заповнення");
            }

            if (firstName.length < 2 || firstName.length > 20) {
                errors.push("Довжина 'Ім'я' повинна бути від 2 до 20 символів");
            }

            if (/\d/.test(firstName)) {
                errors.push("Поле 'Ім'я' не повинно містити цифри");
            }

            firstNameFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
    {
        field: patronymicField,
        validate: () => {
            const patronymic = patronymicField.value.trim();
            const errors = [];

            if (patronymic.length > 0) {
                if (patronymic.length < 2 || patronymic.length > 20) {
                    errors.push("Довжина 'По-батькові' повинна бути від 2 до 20 символів");
                }
            }

            if (/\d/.test(patronymic)) {
                errors.push("Поле 'По-батькові' не повинно містити цифри");
            }

            patronymicFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },

    {
        field: universityNameField,
        validate: () => {
            const universityName = universityNameField.value.trim();
            const errors = [];

            if (universityName.length > 0) {
                if (universityName.length < 2 || universityName.length > 20) {
                    errors.push("Довжина 'Назва університету' повинна бути від 2 до 20 символів");
                }
            }

            if (/\d/.test(universityName)) {
                errors.push("Поле 'Назва університету' не повинно містити цифри");
            }

            universityNameFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },

    {
        field: universityFacultyField,
        validate: () => {
            const universityFaculty = universityFacultyField.value.trim();
            const errors = [];

            if (universityFaculty.length > 0) {
                if (universityFaculty.length < 2 || universityFaculty.length > 20) {
                    errors.push("Довжина 'Факультет' повинна бути від 2 до 20 символів");
                }
            }

            if (/\d/.test(universityFaculty)) {
                errors.push("Поле 'Факультет' не повинно містити цифри");
            }

            universityFacultyFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },

    {
        field: passwordField,
        validate: () => {
            const password = passwordField.value.trim();
            const errors = [];

            if (password === "") {
                errors.push("Поле 'Пароль' є обов'язковим для заповнення");
            }

            if (password.length < 8 || password.length > 20) {
                errors.push("Довжина 'Пароль' повинна бути від 8 до 20 символів");
            }

            if (/\d/.test(password)) {
                errors.push("Поле 'Пароль' не повинно містити цифри");
            }

            passwordFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
    {
        field: confirmPasswordField,
        validate: () => {
            const confirmPassword = confirmPasswordField.value.trim();
            const errors = [];

            if (confirmPassword != passwordField.value) {
                errors.push("Паролі не співпадають.");
            }


            confirmPasswordFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
];

validators.forEach(({ field, validate }) => {
    field.addEventListener("input", () => {
        const allValid = validators.every(({ validate }) => validate());
        submitButton.disabled = !allValid;
    });
});