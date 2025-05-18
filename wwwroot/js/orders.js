// orders.js

document.addEventListener('DOMContentLoaded', function() {
    // Initialize alert dismissal functionality
    const alertCloseButtons = document.querySelectorAll('.alert .close');
    
    alertCloseButtons.forEach(button => {
        button.addEventListener('click', function() {
            const alert = this.closest('.alert');
            alert.classList.remove('show');
            
            setTimeout(() => {
                alert.remove();
            }, 150);
        });
    });
    
    // Initialize order cancellation confirmation
    const cancelButtons = document.querySelectorAll('[data-cancel-order]');
    
    cancelButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to cancel this order?')) {
                e.preventDefault();
            }
        });
    });
});