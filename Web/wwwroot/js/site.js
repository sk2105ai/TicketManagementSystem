// wwwroot/js/site.js
// Common JavaScript functions

// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    // Get all alerts
    var alerts = document.querySelectorAll('.alert');

    // Set timeout to close after 5 seconds
    alerts.forEach(function (alert) {
        setTimeout(function () {
            var closeButton = alert.querySelector('.btn-close');
            if (closeButton) {
                closeButton.click();
            } else {
                alert.style.display = 'none';
            }
        }, 5000);
    });
});