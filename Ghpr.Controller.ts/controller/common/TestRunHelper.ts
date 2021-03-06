﻿///<reference path="./../dto/TestRunDto.ts"/>
///<reference path="./../dto/TestOutputDto.ts"/>
///<reference path="./Color.ts"/>
///<reference path="./DateFormatter.ts"/>

class TestRunHelper {
    static getColorByResult(result: TestResult): string {
        switch (result) {
            case TestResult.Passed:
                return Color.passed;
            case TestResult.Failed:
                return Color.failed;
            case TestResult.Broken:
                return Color.broken;
            case TestResult.Ignored:
                return Color.ignored;
            case TestResult.Inconclusive:
                return Color.inconclusive;
            case TestResult.Unknown:
                return Color.unknown;
            default:
                return "white";
        }
    }

    static getColor(t: TestRunDto): string {
        const result = this.getResult(t);
        return this.getColorByResult(result);
    }
    
    static getResult(t: TestRunDto): TestResult {
        const r = t.result;
        if (r.indexOf("Passed") > -1 || r.indexOf("passed") > -1) {
            return TestResult.Passed;
        }
        if (r.indexOf("Error") > -1 || r.indexOf("Error") > -1) {
            return TestResult.Broken;
        }
        if (r.indexOf("Failed") > -1 || r.indexOf("Failure") > -1 || r.indexOf("failed") > -1 || r.indexOf("failure") > -1) {
            return TestResult.Failed;
        }
        if (r.indexOf("Inconclusive") > -1 || r.indexOf("inconclusive") > -1) {
            return TestResult.Inconclusive;
        }
        if (r.indexOf("Ignored") > -1 || r.indexOf("Skipped") > -1 || r.indexOf("ignored") > -1 || r.indexOf("skipped") > -1
            || r.indexOf("notexecuted") > -1 || r.indexOf("NotExecuted") > -1) {
            return TestResult.Ignored;
        }
        return TestResult.Unknown;
    }

    static getColoredResult(t: TestRunDto): string {
        return `<span class="p-1" style= "background-color: ${this.getColor(t)};" > ${t.result} </span>`;
    }

    static getColoredIns(v: string): string {
        return `<ins class="p-0" style= "background-color: ${Color.passed};text-decoration: none;" >${v}</ins>`;
    }

    static getColoredDel(v: string): string {
        return `<del class="p-0" style= "background-color: ${Color.failed};text-decoration: none;" >${v}</del>`;
    }

    static getOutput(t: TestOutputDto): string {
        return t.output === "" ? "-" : t.output;
    }

    static getExtraOutput(t: TestOutputDto): string {
        return t.suiteOutput === "" ? "-" : t.suiteOutput;
    }

    static getMessage(t: TestRunDto): string {
        return t.testMessage === "" ? "-" : t.testMessage;
    }

    static getPriority(t: TestRunDto): string {
        return t.priority === "" || t.priority === undefined || t.priority === null ? "-" : t.priority;
    }
    
    static getTestType(t: TestRunDto): string {
        return t.testType === "" || t.testType === undefined || t.testType === null ? "-" : t.testType;
    }

    static getDescription(t: TestRunDto): string {
        return (t.description === "" || t.description === undefined) ? "-" : t.description;
    }

    static getStackTrace(t: TestRunDto): string {
        return t.testStackTrace === "" ? "-" : t.testStackTrace;
    }

    static getCategories(t: TestRunDto): string {
        if (t.categories === undefined) {
            return "-";
        }
        return t.categories.length <= 0 ? "-" : t.categories.join(", ");
    }
}