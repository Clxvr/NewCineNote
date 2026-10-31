export function printContent(element) {
    const content = element.__quill.root.innerHTML;

    const printWindow = window.open('', '_blank', 'width=900,height=700');

    if (!printWindow) {
        alert('Разрешите всплывающие окна для печати.');
        return;
    }

    printWindow.document.open();

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

                h1, h2, h3 {
                    text-align: left;
                    text-indent: 0;
                }

                ol, ul {
                    padding-left: 1cm;
                }
            </style>
        </head>

        <body>
            <h2>Синопсис</h2>
            <main>${content}</main>
        </body>
        </html>
    `);

    printWindow.document.close();

    printWindow.onload = function () {
        printWindow.focus();
        printWindow.print();
    };
}