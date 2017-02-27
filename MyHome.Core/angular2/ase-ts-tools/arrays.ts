export class Arrays {
    remove = function<T>(array: T[], search: (item: T) => boolean ) {
        if (array == null) {
            return -1;
        }
        for(let i = 0; i < array.length; i++) {
            if (search(array[i])) {
                array.splice(i, 1);
                return i;
            }
        }
        return -1;
    }

    find = function<T>(array: T[], search: (item: T) => boolean ) {
        if (array == null) {
            return -1;
        }
        for(let i = 0; i < array.length; i++) {
            if (search(array[i])) {
                return array[i];
            }
        }
        return null;
    }

    recursive = function<T>(array: T[], search: (item: T, index: number) => boolean ): boolean {
        if (array == null) {
            return false;
        }
        for(let i = 0; i < array.length; i++) {
            if (search(array[i], i)) {
                return true;
            }
        }
        return false;
    }
}