import $ from 'jquery';

let i = 1;
export default () => {
    $('#CounterButton').on('click', () => {
        $('#CounterDiv').text(`Counter: ${i++}`);
    });
}