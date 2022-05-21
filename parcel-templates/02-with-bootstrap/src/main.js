// Import all plugins
import * as bootstrap from 'bootstrap';

// Or import only needed plugins
import { Tooltip as Tooltip, Toast as Toast, Popover as Popover } from 'bootstrap';

// Or import just one
import Alert from '../node_modules/bootstrap/js/dist/alert';

export default () => {
    document.getElementById('message').innerText = "Hello World!";

    document.getElementById('ToastButton').onclick = () => {
        var myAlert = document.getElementById('ToastMessage');//select id of toast
        let bsAlert = new bootstrap.Toast(myAlert);
        bsAlert.show();//show it
    };

}