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
        required: "Email is required.",
        invalid: "Email address is invalid."
    },
    lastName: {
        required: "Last name is required.",
        noDigits: "Last name can`t contain digits."
    },
    firstName: {
        required: "First name is required.",
        noDigits: "First name can`t contain digits."
    },
    patronymic: {
        noDigits: "Patronymic can`t contain digits."
    },
    universityName: {
        noDigits: "University name can`t contain digits."
    },
    universityFaculty: {
        noDigits: "University faculty can`t contain digits."
    },
    password: {
        required: "Password is required."
    },
    confirmPassword: {
        dontMatch: "Passwords do not match."
    },
    common: {
        minLength: "Try something longer.",
        maxLength: "Try something shorter"
    },

};

const validators = [
    {
        field: userTagField,
        validate: () => {
            const userTag = userTagField.value.trim();
            const errors = [];
            if (userTag.length > 0) {
                if (userTag.length < 2) {
                    errors.push(errorMessages.common.minLength);
                }
                else if (userTag.length > 20) {
                    errors.push(errorMessages.common.maxLength);
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
         
            if (email === "") {
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
                errors.push(errorMessages.lastName.required);
            }

            if (lastName.length < 2) {
                errors.push(errorMessages.common.minLength);
            }
            else if (lastName.length > 20)
            {
                errors.push(errorMessages.common.maxLength);
            }

            if (/\d/.test(lastName)) {
                errors.push(errorMessages.lastName.noDigits);
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
                errors.push(errorMessages.firstName.required);
            }

            if (firstName.length < 2){
                errors.push(errorMessages.common.minLength);
            }
            else if (firstName.length > 20) {
                errors.push(errorMessages.common.maxLength);
            }

            if (/\d/.test(firstName)) {
                errors.push(errorMessages.firstName.noDigits);
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
                if (patronymic.length < 2 ) {
                    errors.push(errorMessages.common.minLength);
                }
                else if (patronymic.length > 20) {
                    errors.push(errorMessages.common.maxLength);
                }
            }

            if (/\d/.test(patronymic)) {
                errors.push(errorMessages.patronymic.noDigits);
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
                if (universityName.length < 2) {
                    errors.push(errorMessages.common.minLength);
                }
                else if (universityName.length > 20) {
                    errors.push(errorMessages.common.maxLength);
                }
            }

            if (/\d/.test(universityName)) {
                errors.push(errorMessages.universityName.noDigits);
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
                if (universityFaculty.length < 2) {
                    errors.push(errorMessages.common.minLength);
                }
                else if (universityFaculty.length > 20) {
                    errors.push(errorMessages.common.maxLength);
                }
            }

            if (/\d/.test(universityFaculty)) {
                errors.push(errorMessages.universityFaculty.noDigits);
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
                errors.push(errorMessages.password.required);
            }

            if (password.length > 0) {
                if (password.length < 2) {
                    errors.push(errorMessages.common.minLength);
                }
                else if (password.length > 20) {
                    errors.push(errorMessages.common.maxLength);
                }
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
                errors.push(errorMessages.confirmPassword.dontMatch);
            }

            confirmPasswordFieldErrorMessage.innerHTML = errors.length > 0 ? errors[0] : "";

            return errors.length === 0;
        },
    },
];

validators.forEach(({ field, validate }) => {
    field.addEventListener("input", () => {
        let allValid = true;
        for (let i = 0; i < validators.length; i++) {
            if (!validators[i].validate()) {
                allValid = false;
            }
        }
        console.log(allValid);
        submitButton.disabled = !allValid;
    });
});