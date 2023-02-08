if(typeof(axm) === "undefined"){
    axm = {
        __namespace:true
    };
}
if(typeof(axm.tableName)==="undefined"){
    axm.tableName = {
        __namespace : true
    };
}

axm.tableName.Event = (function () {
    var onLoad = function(executionContext){
        var formContext = executionContext.getFormContext();
        //OnChange Events
        formContext.getControl("axm_fromDate",formContext).getAttribute().addOnChange(contractFromDate);
        //OnLoad Events

    };
    var onSave = function(executionContext){
        //OnSave Events
    };
    //Your functions
    var contractFromDate = function(executionContext){
        var formContext = executionContext.getFormContext();
        debugger;
    }


    return {
        OnLoad: onLoad,
        OnSave: onSave
    }
})();