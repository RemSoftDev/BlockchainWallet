import { WatchlistModel } from '../shared/models/watchlistModel';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'orderBy'
})
export class SortingWatchlistPipe implements PipeTransform {

  transform(companies: WatchlistModel[], path: string[], order: number = 1): WatchlistModel[] {

    // Check if is not null
    if (!companies || !path || !order) return companies;

    return companies.sort((a: WatchlistModel, b: WatchlistModel) => {
      // We go for each property followed by path
      path.forEach(property => {
        a = a[property];
        b = b[property];
      })

      // Order * (-1): We change our order
      return a > b ? order : order * (- 1);
    })
  }

}
