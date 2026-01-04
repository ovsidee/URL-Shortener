import { Component } from '@angular/core';

@Component({
  standalone: false, // Ensure this is false too
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  title = 'ClientApp';
}
