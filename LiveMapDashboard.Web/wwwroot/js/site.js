// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showAlert(type, message) {
    const alert = document.createElement('div');
    alert.classList.add('alert');
    alert.role = 'alert';
    alert.innerHTML = message;

    switch (type) {
        case 'danger':
        case 'error':
            alert.classList.add('alert-danger');
            break;
        case 'warning':
            alert.classList.add('alert-warning');
            break;
        case 'success':
            alert.classList.add('alert-success');
            break;
        case 'info':
        default:
            alert.classList.add('alert-info');
            break;
    }

    setTimeout(() => {
        alert.classList.add('fade-out');

        // After 4 seconds (1 second for fade-out), remove the alert.
        setTimeout(() => {
            alert.remove();
        }, 1000);
    }, 4000);

    document.querySelector('#alertContainer').append(alert);
}