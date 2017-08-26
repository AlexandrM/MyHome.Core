import { Injectable } from '@angular/core';
import { Arrays } from './arrays';
import { JSONAdv } from './json';

@Injectable()
export class ASE {
    static a = new Arrays();
    static json = new JSONAdv();

    padLeft(num: number, size: number): string {
        var s = num + '';
        while (s.length < size) {
            s = '0' + s;
        }
        return s;
    }
}
