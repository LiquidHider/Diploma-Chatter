function logOut() {
    fetch('Chat/Logout', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            window.location.href = '/';
            console.log(response.json());
        })
        .catch(error => console.error(error));
}