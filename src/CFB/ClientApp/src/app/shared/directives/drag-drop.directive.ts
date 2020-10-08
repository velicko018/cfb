import { Directive, Output, Input, EventEmitter, HostBinding, HostListener } from '@angular/core';

@Directive({
    selector: '[appDragDrop]'
})
export class DragDropDirective {
    @Output() onFileDropped = new EventEmitter<any>();

    @HostBinding('style.background-color')
    public background = '#fff';
    @HostBinding('style.opacity')
    public opacity = '1';

    @HostListener('dragover', ['$event'])
    public onDragOver(event) {
        event.preventDefault();
        event.stopPropagation();
        this.background = '#9ecbec';
        this.opacity = '0.8'
    };

    @HostListener('dragleave', ['$event'])
    public onDragLeave(event) {
        event.preventDefault();
        event.stopPropagation();
        this.background = '#fff'
        this.opacity = '1'
    }

    @HostListener('drop', ['$event'])
    public onDrop(event) {        
        event.preventDefault();
        event.stopPropagation();
        this.background = '#f5fcff'
        this.opacity = '1'
        let files = event.dataTransfer.files;
        if (files.length > 0) {
            this.onFileDropped.emit(files)
        }
    }
}
