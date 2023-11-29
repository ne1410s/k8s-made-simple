import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";

import { UploadFileComponent } from "./components/upload-file/upload-file.component";
import { ConvertPdfComponent } from "./components/convert-pdf/convert-pdf.component";

@NgModule({
  declarations: [ UploadFileComponent, ConvertPdfComponent ],
  imports: [ CommonModule, FormsModule, HttpClientModule ],
  exports: [ UploadFileComponent, ConvertPdfComponent ],
})
export class FileManagerModule {}