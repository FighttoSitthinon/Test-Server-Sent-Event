// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const requestForm = document.getElementById('request-form');
const messageInput = document.getElementById('request-message');
const submitButton = document.getElementById('submit-request');
const connectionStatus = document.getElementById('connection-status');
const submitStatus = document.getElementById('submit-status');
const messageData = document.getElementById('message-data');

if (requestForm && messageInput && submitButton && connectionStatus && submitStatus && messageData) {
    const eventSource = new EventSource('/Home/GetEvents');
    let isSubmitting = false;

    eventSource.onopen = function () {
        connectionStatus.textContent = 'Connected to event stream';
        submitButton.disabled = isSubmitting;
    };

    eventSource.onerror = function () {
        connectionStatus.textContent = 'Reconnecting to event stream...';
        submitButton.disabled = true;
    };

    eventSource.onmessage = function (event) {
        const line = document.createElement('div');
        line.textContent = event.data;
        messageData.appendChild(line);
        messageData.scrollTop = messageData.scrollHeight;
    };

    requestForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        isSubmitting = true;
        submitButton.disabled = true;
        submitStatus.textContent = '';

        try {
            const response = await fetch('/Home/SubmitRequest', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: messageInput.value })
            });

            if (!response.ok) {
                throw new Error(`Request failed (${response.status})`);
            }

            messageInput.value = '';
            submitStatus.textContent = 'Request sent.';
        } catch (error) {
            submitStatus.textContent = error.message;
        } finally {
            isSubmitting = false;
            submitButton.disabled = eventSource.readyState !== EventSource.OPEN;
        }
    });
}
