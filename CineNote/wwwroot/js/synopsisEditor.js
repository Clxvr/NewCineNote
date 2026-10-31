
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

export function printPage() {
    window.print();
}
