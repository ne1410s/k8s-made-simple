import { Component } from '@angular/core';
import { FileManagerService } from '../../services/file-manager.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.scss']
})
export class UploadFileComponent {
  
  public clean?: Observable<boolean>;
  
  private file?: File;

  constructor(private fileManagerService: FileManagerService)
  { }

  onFileChange(event: Event) {
    const files = (event.target as HTMLInputElement).files || [];
    this.file = files.length > 0 ? files[0] : undefined;
  }

  submit() {
    if (this.file) {
      console.log('submitting!');
      this.clean = this.fileManagerService.scan(this.file!);
    }
  }
}
