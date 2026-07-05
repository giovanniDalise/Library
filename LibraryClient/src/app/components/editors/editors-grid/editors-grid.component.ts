import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Editor } from '../../../models/editor/editor/editor';

@Component({
  selector: 'app-editors-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './editors-grid.component.html',
  styleUrl: './editors-grid.component.scss'
})
export class EditorsGridComponent {
  //@Input ricevo dal padre (page componet)
  @Input() editors: Editor[] = [];
  
  @Output() viewDetail = new EventEmitter<number>();

  onViewDetail(editorId: number): void { this.viewDetail.emit(editorId); } 

}
