export function initializeEditor(element, initialContent) {
    if (element.__quill) {
        return;
    }

    const quill = new Quill(element, {
        theme: 'snow',
        modules: {
            toolbar: {
                container: [
                    [{
                        'header': [
                            false,
                            '1',
                            '2',
                            '3'
                        ]
                    }],
                    ['bold', 'italic', 'underline'],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    ['clean']
                ]
            }
        }
    });

    element.__quill = quill;
    quill.root.innerHTML = initialContent || '';

    // Переименовываем пункты выпадающего списка
    setTimeout(() => {
        const pickerItems = element.parentNode.querySelectorAll('.ql-picker.ql-header .ql-picker-item');
        pickerItems.forEach(item => {
            const value = item.getAttribute('data-value');
            if (value === null) item.textContent = 'Обычный текст (Действие)';
            if (value === '1') item.textContent = 'Заголовок сцены';
            if (value === '2') item.textContent = 'Имя персонажа';
            if (value === '3') item.textContent = 'Ремарка / Диалог';
        });
    }, 50);
}

export function getContent(element) {
    if (!element.__quill) {
        return '';
    }
    return element.__quill.root.innerHTML;
}

export function printContent(element) {
    if (!element || !element.__quill) {
        alert('Редактор не загружен.');
        return;
    }

    const content = element.__quill.root.innerHTML;
    const printWindow = window.open('', '_blank');

    if (!printWindow) {
        alert('Разрешите всплывающие окна для сайта.');
        return;
    }

    printWindow.document.write(`
        <!DOCTYPE html>
        <html lang="ru">
        <head>
            <meta charset="UTF-8">
            <title>Печать сцены</title>
            <style>
                @page { size: A4; margin: 2.5cm 2.5cm 2.5cm 3.8cm; }
                body { font-family: "Courier New", Courier, monospace; font-size: 12pt; color: black; }
                p { margin-bottom: 1em; }
                h1 { font-size: 12pt; text-transform: uppercase; margin-top: 1.5em; }
                h2 { font-size: 12pt; text-transform: uppercase; margin-left: 5.7cm; margin-top: 1.5em; }
                h3 { font-size: 12pt; font-style: italic; margin-left: 3.5cm; }
            </style>
        </head>
        <body>
            ${content}
        </body>
        </html>
    `);

    printWindow.document.close();
    printWindow.onload = function () {
        printWindow.focus();
        printWindow.print();
    };
}