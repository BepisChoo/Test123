// Toggle password visibility
function togglePassword() {
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    const eyeIcon = document.getElementById('eye-icon');

    if (passwordInput) {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        if (confirmPasswordInput) {
            confirmPasswordInput.setAttribute('type', type);
        }

        // Toggle eye icon
        if (type === 'text') {
            eyeIcon.innerHTML = '<path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line>';
        } else {
            eyeIcon.innerHTML = '<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle>';
        }
    }
}

// Toggle role-specific fields in registration form
function toggleRoleFields() {
    const roleSelect = document.getElementById('roleSelect');
    const doctorFields = document.getElementById('doctorFields');
    const patientFields = document.getElementById('patientFields');

    if (!roleSelect) return;

    const selectedRole = roleSelect.value;

    // Hide all role-specific fields first
    if (doctorFields) doctorFields.style.display = 'none';
    if (patientFields) patientFields.style.display = 'none';

    // Show relevant fields based on role
    if (selectedRole === 'Doctor' && doctorFields) {
        doctorFields.style.display = 'block';
    } else if (selectedRole === 'Patient' && patientFields) {
        patientFields.style.display = 'block';
    }
}

// Form validation
document.addEventListener('DOMContentLoaded', function () {
    const registerForm = document.getElementById('registerForm');

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            const password = document.getElementById('password');
            const confirmPassword = document.getElementById('confirmPassword');

            if (password && confirmPassword) {
                if (password.value !== confirmPassword.value) {
                    e.preventDefault();
                    alert('Passwords do not match!');
                    return false;
                }

                if (password.value.length < 6) {
                    e.preventDefault();
                    alert('Password must be at least 6 characters long!');
                    return false;
                }
            }
        });
    }

    // Auto-hide success messages after 5 seconds
    const successAlerts = document.querySelectorAll('.alert-success');
    successAlerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        }, 5000);
    });
});

// Real-time password strength indicator (optional enhancement)
function checkPasswordStrength() {
    const password = document.getElementById('password');
    if (!password) return;

    password.addEventListener('input', function () {
        const value = this.value;
        let strength = 0;

        if (value.length >= 6) strength++;
        if (value.length >= 10) strength++;
        if (/[a-z]/.test(value) && /[A-Z]/.test(value)) strength++;
        if (/\d/.test(value)) strength++;
        if (/[^a-zA-Z0-9]/.test(value)) strength++;
        // You can add a visual indicator here if needed
        console.log('Password strength:', strength);
    });