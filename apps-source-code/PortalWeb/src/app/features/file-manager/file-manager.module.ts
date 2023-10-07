import { NgModule } from "@angular/core";
import { UploadFileComponent } from "./components/upload-file.component";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";

@NgModule({
  declarations: [ UploadFileComponent ],
  imports: [ CommonModule, FormsModule, HttpClientModule ],
  exports: [ UploadFileComponent ],
})
export class FileManagerModule {}