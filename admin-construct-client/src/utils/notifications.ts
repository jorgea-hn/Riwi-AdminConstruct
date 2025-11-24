import Swal from 'sweetalert2';

// Toast configuration
const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer);
        toast.addEventListener('mouseleave', Swal.resumeTimer);
    },
});

export const notifications = {
    success: (message: string) => {
        Toast.fire({
            icon: 'success',
            title: message,
            background: '#0d1b2a',
            color: '#ffffff',
            iconColor: '#FF8C00',
        });
    },

    error: (message: string) => {
        Toast.fire({
            icon: 'error',
            title: message,
            background: '#0d1b2a',
            color: '#ffffff',
            iconColor: '#dc3545',
        });
    },

    warning: (message: string) => {
        Toast.fire({
            icon: 'warning',
            title: message,
            background: '#0d1b2a',
            color: '#ffffff',
            iconColor: '#ffc107',
        });
    },

    info: (message: string) => {
        Toast.fire({
            icon: 'info',
            title: message,
            background: '#0d1b2a',
            color: '#ffffff',
            iconColor: '#17a2b8',
        });
    },

    confirm: async (title: string, text: string) => {
        const result = await Swal.fire({
            title,
            text,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#FF8C00',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Sí, confirmar',
            cancelButtonText: 'Cancelar',
            background: '#0d1b2a',
            color: '#ffffff',
        });
        return result.isConfirmed;
    },
};
