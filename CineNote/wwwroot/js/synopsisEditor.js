
export function initializeEditor(element, initialContent) {
    if (element.__quill) {
        return;
    }

    const quill = new Quill(element, {
        theme: 'snow',
        modules: {
            toolbar: [
                [{ header: [1, 2, 3, false] }],
                ['bold', 'italic', 'underline'],
                [{ list: 'ordered' }, { list: 'bullet' }],
                [{ align: [] }],
                ['link'],
                ['clean']
            ]
        }
    });

    element.__quill = quill;
    quill.root.innerHTML = initialContent || '';
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
            <title>Синопсис</title>
            <style>
                @page {
                    size: A4;
                    margin: 20mm;
                }

                body {
                    font-family: "Times New Roman", serif;
                    font-size: 14pt;
                    line-height: 1.5;
                    color: black;
                }

                h2 {
                    text-align: center;
                    font-size: 16pt;
                    margin-bottom: 25px;
                }

                p {
                    text-align: justify;
                    text-indent: 1.25cm;
                    margin: 0;
                }
            </style>
        </head>
        <body>
            <h2>Синопсис</h2>
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
