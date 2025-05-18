/**
 * Manageproduct.js
 * JavaScript functionality for the Product Management dashboard
 */

// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Initialize variables
    let productToDelete = null;
    let currentPage = 1;
    const itemsPerPage = 10; // You can adjust this value

    // Elements
    const searchInput = document.getElementById('searchProducts');
    const productTableBody = document.getElementById('productTableBody');
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
    const paginationElement = document.getElementById('pagination');
    const prevPageBtn = document.getElementById('prevPage');
    const nextPageBtn = document.getElementById('nextPage');

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    const tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    /**
     * Toast notification function
     * @param {string} title - The title of the notification
     * @param {string} message - The message to display
     * @param {string} type - The type of notification (success, error, warning, info)
     */
    function showToast(title, message, type = 'success') {
        const toastElement = document.querySelector('.toast');
        const toastTitle = document.getElementById('toast-title');
        const toastMessage = document.getElementById('toast-message');

        // Set toast content
        toastTitle.textContent = title;
        toastMessage.textContent = message;

        // Set toast type (color)
        toastElement.classList.remove('bg-success', 'bg-danger', 'bg-warning', 'bg-info');

        // Add appropriate background class based on type
        switch (type) {
            case 'success':
                toastElement.querySelector('.toast-header').style.borderBottom = '2px solid #2ecc71';
                break;
            case 'error':
                toastElement.querySelector('.toast-header').style.borderBottom = '2px solid #e74c3c';
                break;
            case 'warning':
                toastElement.querySelector('.toast-header').style.borderBottom = '2px solid #f39c12';
                break;
            case 'info':
                toastElement.querySelector('.toast-header').style.borderBottom = '2px solid #3498db';
                break;
        }

        // Create and show the toast
        const toast = new bootstrap.Toast(toastElement);
        toast.show();
    }

    /**
     * Search functionality
     */
    searchInput.addEventListener('input', function () {
        const searchTerm = this.value.toLowerCase();
        const rows = productTableBody.querySelectorAll('tr');

        rows.forEach(row => {
            const productName = row.querySelector('.product-name').textContent.toLowerCase();
            if (productName.includes(searchTerm)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });

        // If search is active, hide pagination
        if (searchTerm) {
            document.querySelector('.card-footer').style.display = 'none';
        } else {
            document.querySelector('.card-footer').style.display = '';
            updatePagination(); // Reset pagination
        }
    });

    
    /**
     * Update pagination based on current data
     */
    function updatePagination() {
        // Get all visible rows
        const visibleRows = Array.from(productTableBody.querySelectorAll('tr'))
            .filter(row => row.style.display !== 'none');

        // Calculate total pages
        const totalPages = Math.ceil(visibleRows.length / itemsPerPage);

        // Update pagination UI
        let paginationHTML = '';

        // Prev button
        paginationHTML += `
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" aria-label="Previous" id="prevPage">
                    <span aria-hidden="true">&laquo;</span>
                </a>
            </li>
        `;

        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            paginationHTML += `
                <li class="page-item ${currentPage === i ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `;
        }

        // Next button
        paginationHTML += `
            <li class="page-item ${currentPage === totalPages || totalPages === 0 ? 'disabled' : ''}">
                <a class="page-link" href="#" aria-label="Next" id="nextPage">
                    <span aria-hidden="true">&raquo;</span>
                </a>
            </li>
        `;

        paginationElement.innerHTML = paginationHTML;

        // Show/hide rows based on current page
        showCurrentPageItems();

        // Add event listeners to new pagination elements
        addPaginationEventListeners();
    }

    /**
     * Show items for the current page
     */
    function showCurrentPageItems() {
        const rows = productTableBody.querySelectorAll('tr');
        const startIndex = (currentPage - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;

        let visibleIndex = 0;
        rows.forEach(row => {
            if (row.style.display !== 'none') {
                if (visibleIndex >= startIndex && visibleIndex < endIndex) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
                visibleIndex++;
            }
        });
    }

    /**
     * Add event listeners to pagination elements
     */
    function addPaginationEventListeners() {
        // Page number clicks
        const pageLinks = document.querySelectorAll('.page-link[data-page]');
        pageLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                currentPage = parseInt(this.dataset.page);
                updatePagination();
            });
        });

        // Previous page button
        const prevPageBtn = document.getElementById('prevPage');
        if (prevPageBtn) {
            prevPageBtn.addEventListener('click', function (e) {
                e.preventDefault();
                if (currentPage > 1) {
                    currentPage--;
                    updatePagination();
                }
            });
        }

        // Next page button
        const nextPageBtn = document.getElementById('nextPage');
        if (nextPageBtn) {
            nextPageBtn.addEventListener('click', function (e) {
                e.preventDefault();
                const totalPages = document.querySelectorAll('.page-link[data-page]').length;
                if (currentPage < totalPages) {
                    currentPage++;
                    updatePagination();
                }
            });
        }
    }

    // Initialize pagination on page load
    updatePagination();
});