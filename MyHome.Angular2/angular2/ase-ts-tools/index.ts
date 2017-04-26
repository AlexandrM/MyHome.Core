import { Injectable } from '@angular/core';
import { Arrays } from './arrays';

@Injectable()
export class ASE {
    static a = new Arrays();

    padLeft(num: number, size: number): string {
        var s = num + '';
        while (s.length < size) {
            s = '0' + s;
        }
        return s;
    }
}
