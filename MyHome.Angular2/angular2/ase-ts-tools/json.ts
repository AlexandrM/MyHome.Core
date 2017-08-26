export class JSONAdv {

	public stringify(model: any): string {
		let seen = [];

        return JSON.stringify(model, function(key, val) {
	        if (val != null && typeof val == "object") {
				if (seen.indexOf(val) >= 0) {
        	            return;
                }
                seen.push(val);
            }
            return val;
        });
	}
}