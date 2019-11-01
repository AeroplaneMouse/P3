var current_group = null;
var dbtags = null;
var tags = new Array();


$( document ).ready(function() {

    $(document).on("click", ".change-page" , function() {
        var page = "pages/"+$(this).data("page");
        $('#content').load(page);
    });

    $(document).on("dblclick", ".change-page-dbl" , function() {
        var page = "pages/"+$(this).data("page");
        $('#content').load(page);
    });

    $('#content').load("pages/assets.html");

    $.getJSON('data.json', function(data) {
        dbtags = data.items;
        updateTaglist();
    });

    $( "#tag-input" ).keyup(function() {
        checkInput(this.value);
    });

    $(document).keyup(function(e) {
        if(e.key === "Enter"){
            addTag($('#tag-input').val().toLowerCase());
            clearTagField();
        }
    });

    $(document).keydown(function(e) {
        if (e.key === "Escape" || (e.keyCode == 8 && $('#tag-input').val() == "")) {
            changeGroup(null);
        }
    });

    $('#tag-input').focus();
});

function checkInput(input){
    if(input.length > 0 && input != " " && input != ":"){
        matchValue = isTagGroup(input);
        console.log(matchValue);

        if((matchValue || input.endsWith(":") || input.endsWith(" ")) && this.current_group == null){
            group = input.replace(' ', '');
            group = group.replace(':', '');
            changeGroup(group);
        }
    }else{
        clearTagField();
    }
}

function changeGroup(group){
    this.current_group = (group == null ? null : group.toLowerCase());
    $('.tag-group > span').text(this.current_group != null ? this.current_group : "#");
    clearTagField();
    updateTaglist();
}

function addTag(tag){
    tag = tag.trim();
    if(tag != ""){
        tag_text = this.current_group != null ? this.current_group+":"+tag : tag;
        $('#tags').append('<span class="badge badge-secondary">'+tag_text+'</span>');
        console.log(tag_text);
    }
}

function clearTagField(){
    $('#tag-input').val("");
}

function outputIt(){
    jQuery.each( this.dbtags, function( i, val ) {
        console.log(val.label);
    });
}

function updateTaglist(){
    $('#taglist').html("");
    group = this.current_group;
    if(this.current_group == null){
        jQuery.each( this.dbtags, function( i, val ) {
            if(val.parent_id == 0){
                $('#taglist').append('<option value="'+val.label+'">');
            }
        });
    }else{
        lparent_id = 0;
        jQuery.each( this.dbtags, function( i, val ) {
            if(self.group == val.label){
                self.lparent_id = val.id;
            }
        });

        if(lparent_id > 0){
            jQuery.each( this.dbtags, function( i, val ) {
                if(val.parent_id == self.lparent_id){
                    $('#taglist').append('<option value="'+val.label+'">');
                }
            });
        }
    }
}

function isTagGroup(input){
    doinput = input;
    returnValue = false;
    if(this.current_group == null){
        jQuery.each( this.dbtags, function( i, val ) {
            if(val.parent_id == 0 && self.doinput == val.label){
                console.log("Group match found!");
                self.returnValue = true;
                return false;
            }
        });
    }
    return returnValue;
}