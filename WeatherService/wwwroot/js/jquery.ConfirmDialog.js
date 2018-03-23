(function($)
{
  var methods =
  {
    show: function()
    {
      var opts = $(this).data("jquery.confirmDialog.options");
      var id = $(this).data("jquery.confirmDialog.id");
      var el = $('#' + id);

      $(el).find('h5.modal-title').text(opts.title);
      $(el).find('div.modal-body').text(opts.text);

      $(el).find('button:eq(1)')
        .unbind('click')
        .text(opts.confirmText)
        .attr('class', 'btn ' + opts.confirmClass);

      $(el).find('button:eq(2)')
        .text(opts.cancelText)
        .attr('class', 'btn ' + opts.cancelClass);

      if(opts.onConfirm)
      {
        $(el).find('button:eq(1)').bind('click', function()
        {
          if(opts.onConfirm())
          {
            $(el).modal('hide');
          }
        });
      }

      $(el).modal('show');

      return null;
    },
    hide: function()
    {
      var id = $(this).data("jquery.confirmDialog.id");
      var el = $('#' + id);

      $(el).modal('hide');
    }
  };

  function initializePlugin(obj, options)
  {
    var sel = $(obj);
    var opts = $(obj).data('jquery.confirmDialog.options');

    if(!opts)
    {
      opts = {title: 'Confirm',
              text: 'To be or not to be?',
              confirmText: 'Yes',
              confirmClass: 'btn-primary',
              cancelText: 'Cancel',
              cancelClass: 'btn-secondary',
              onConfirm: null};
    }

    opts = $.extend(opts, options);

    var sel = $(obj);

    sel.data("jquery.confirmDialog.options", opts);

    if(!$(this).data("jquery.confirmDialog.id"))
    {
      var el = $('<div class="modal fade" id="confirmDialog" tabindex="-1" role="dialog">' +
        '  <div class="modal-dialog" role="document">' +
        '    <div class="modal-content">' +
        '      <div class="modal-header">' +
        '        <h5 class="modal-title"></h5>' +
        '        <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
        '          <span aria-hidden="true">&times;</span>' +
        '        </button>' +
        '      </div>' +
        '      <div class="modal-body"></div>' +
        '      <div class="modal-footer">' +
        '        <button type="button">' + opts.confirmText + '</button>' +
        '        <button type="button" data-dismiss="modal">' + opts.cancelText + '</button>' +
        '      </div>' +
        '    </div>' +
        '  </div>' +
        '</div>')
      .generateId()
      .appendTo(sel);

      sel.data("jquery.confirmDialog.id", $(el).attr('id'));
    }
  }

  $.fn.confirmDialog = function(args)
  {
    if(methods[args])
    {
      var parameters = Array.prototype.slice.call(arguments, 1);

      return this.each(function()
      {
        if($(this).data("jquery.confirmDialog.id"))
        {
          methods[args].apply(this, parameters);

          return this;
        }
        else
        {
          return null;
        }
      });
    }
    else if(typeof args === 'object' || !args)
    {
      return this.each(function()
      {
        initializePlugin(this, args);
      });
    }
  }
}(jQuery));
