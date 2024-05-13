import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService, PagedResultDto } from '@abp/ng.core';
import { OrdersService, ProductSalesDto } from '@proxy/orders';
import { Subject, takeUntil } from 'rxjs';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ProductsService } from '@proxy/catalog/products';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  private ngUnsubscribe = new Subject<void>();
  public items: ProductSalesDto[] = [];
  public blockedPanel: boolean = false; // Để hiển thị trạng thái loading

  // Paging variables
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;
  // Filter
  keyword: string = '';

  // Date range filter
  startDate: string;
  endDate: string;

  // Image
  public thumbnailImage;
  public thumbnailImageUrl: SafeUrl;

  public chartData: any;
  public chartOptions: any;

  totalQuantitySold: number = 0;
  totalRevenue: number = 0;

  constructor(
    private authService: AuthService,
    private productService: ProductsService,
    private orderService: OrdersService, // Inject OrdersService
    private sanitizer: DomSanitizer,
  ) {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit(): void {
    this.loadData();
  }

  prepareChartData() {
    const labels = this.items.map(item => item.productName);
    const revenues = this.items.map(item => item.totalRevenue);
    const quantities = this.items.map(item => item.quantitySold);
  
    this.chartData = {
      labels: labels,
      datasets: [
        {
          label: 'Total Revenue',
          data: revenues,
          backgroundColor: '#42A5F5',
          yAxisID: 'y-axis-revenue',
          type: 'bar'
        },
        {
          label: 'Quantity Sold',
          data: quantities,
          borderColor: '#FFA726',
          backgroundColor: '#FFA726',
          yAxisID: 'y-axis-quantity',
          type: 'line',
          fill: false
        }
      ]
    };
  
    this.chartOptions = {
      responsive: true,
      scales: {
        x: { // Cập nhật từ 'xAxes' sang 'x'
          scaleLabel: {
            display: true,
            labelString: 'Products'
          }
        },
        y: { // Cập nhật từ 'yAxes' sang 'y'
          'y-axis-revenue': {
            type: 'linear',
            position: 'left',
            scaleLabel: {
              display: true,
              labelString: 'Total Revenue ($)'
            },
            ticks: {
              beginAtZero: true
            }
          },
          'y-axis-quantity': {
            type: 'linear',
            position: 'right',
            scaleLabel: {
              display: true,
              labelString: 'Quantity Sold'
            },
            ticks: {
              beginAtZero: true
            }
          }
        }
      }
    };
  }

  login() {
    this.authService.navigateToLogin(); // Chuyển hướng người dùng đến trang đăng nhập
  }

  loadData() {
    this.toggleBlockUI(true);
    this.orderService
      .getProductSalesStatisticsByTime(
        {
          keyword: this.keyword,
          maxResultCount: this.maxResultCount,
          skipCount: this.skipCount,
        },
        this.startDate,
        this.endDate
      )
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<ProductSalesDto>) => {
          this.items = response.items;
          this.totalCount = response.totalCount;
          this.items.forEach(product => {
            if (product.thumbnailPicture) {
                this.loadThumbnail(product);
            }
          });
          this.calculateTotals();
          this.prepareChartData();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private toggleBlockUI(enabled: boolean) {
    this.blockedPanel = enabled;
    if (!enabled) {
      setTimeout(() => this.blockedPanel = false, 1000); // Đảm bảo rằng UI sẽ được mở khóa sau một khoảng thời gian
    }
  }

  pageChanged(event: any): void {
    this.skipCount = event.first;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  loadThumbnail(product: ProductSalesDto) {
    if (!product.thumbnailPicture) {
      console.log('No thumbnail available for this product.');
      return;
    }
  
    this.productService.getThumbnailImage(product.thumbnailPicture)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: string) => {
          const fileExt = product.thumbnailPicture.split('.').pop();
          product.safeThumbnailUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
            `data:image/${fileExt};base64,${response}`
          );
        },
        error: () => {
          console.error(`Failed to load thumbnail for ${product.thumbnailPicture}`);
          product.safeThumbnailUrl = undefined; // Optionally set a default image in case of error
        }
      });
  }

  resetForm() {
    // Reset search/filter fields
    this.keyword = '';
    this.maxResultCount = 10;  // Giả sử đây là số lượng mặc định bạn muốn tải ban đầu
    this.skipCount = 0;        // Reset về trang đầu tiên
    this.startDate = undefined;
    this.endDate = undefined;

    this.loadData();
  }

  calculateTotals() {
    this.totalQuantitySold = this.items.reduce((acc, item) => acc + item.quantitySold, 0);
    this.totalRevenue = this.items.reduce((acc, item) => acc + item.totalRevenue, 0);
  }

}
