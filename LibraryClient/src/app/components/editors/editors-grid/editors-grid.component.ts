import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Editor } from '../../../models/editor';

@Component({
  selector: 'app-editors-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './editors-grid.component.html',
  styleUrl: './editors-grid.component.scss'
})
export class EditorsGridComponent {
  editors: Editor[] = [
    {
      editorId: 1,
      name: "TestCasaEdi",
      books: []
    }
  ];
  
  @Input() isAdmin = false;
}
