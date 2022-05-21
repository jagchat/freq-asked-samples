// Import all plugins
import * as bootstrap from 'bootstrap';

// Or import only needed plugins
import { Tooltip as Tooltip, Toast as Toast, Popover as Popover } from 'bootstrap';

//import other libs
import $ from 'jquery';

export default () => {
    $('#message').text("Hello World!");
    $('#ToastButton').on("click", () => {
        let bsToast = new bootstrap.Toast($('#ToastMessage'));
        bsToast.show();//show it        
    });

    $('#ModalButton').on("click", () => {
        let bsModal = new bootstrap.Modal($('#ModalMessage'));
        bsModal.show();//show it        
    });

}