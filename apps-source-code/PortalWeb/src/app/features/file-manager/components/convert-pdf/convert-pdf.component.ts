import { Component, OnDestroy } from '@angular/core';
import { FileManagerService } from '../../services/file-manager.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-convert-pdf',
  templateUrl: './convert-pdf.component.html',
  styleUrls: ['./convert-pdf.component.scss']
})
export class ConvertPdfComponent implements OnDestroy {
  
  public $sub?: Subscription;
  public url: string = 'https://me.ne1410s.co.uk';
  public bytes?: number;

  constructor(private fileManagerService: FileManagerService)
  { }

  submit() {
    console.log('submitting!');
    this.$sub = this.fileManagerService.urlToPdf(this.url)
      .subscribe(blob => {
        this.bytes = blob.size;
        let anchor = document.createElement('a');
        anchor.href = window.URL.createObjectURL(blob);
        anchor.setAttribute('download', 'file.pdf');
        document.body.appendChild(anchor);
        anchor.click();
      });
  }

  ngOnDestroy(): void {
    this.$sub?.unsubscribe();
  }
}
