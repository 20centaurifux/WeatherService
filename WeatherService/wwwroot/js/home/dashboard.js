var Dashboard = function(opts)
{
    var options = opts || {};
    var self = this;

    var storeWidgets = function()
    {
        if (options.updateUrl)
        {
            var items = [];

            $('.gridster li').each(function()
            {
                var el = $(this);

                items.push(
                {
                    widgetId: el.attr('data-widget-guid'),
                    x: el.attr('data-col'),
                    y: el.attr('data-row'),
                    filter: el.attr('data-widget-stations').split(',')
                });
            });

            $.ajax(
            {
                url: options.updateUrl,
                type: 'POST',
                data: JSON.stringify(items),
                dataType: 'json',
                contentType: 'application/json'
            }).fail(function(xhr, textStatus, errorThrown) { console.warn(xhr.responseText); });
        }
    }

    var gridster = $(".gridster ul").gridster(
    {
        widget_base_dimensions: [120, 120],
        widget_margins: [10, 10],
        draggable:
        {
            stop: function(e, ui)
            {
                if (options.updateUrl)
                {
                    setTimeout(storeWidgets, 0);
                }
            }
        }
    }).data('gridster');

    this.refresh = function()
    {
        var now = parseInt(new Date().getTime() / 1000, 10);

        $('.gridster li').each(function()
        {
            var el = $(this);
            var lastSync = parseInt(el.attr('data-widget-last-sync'), 10);

            if (!lastSync || now - lastSync >= 180)
            {
                el.attr('data-widget-last-sync', now);

                var url = el.attr('data-widget-url') + '?s=' + el.attr('data-widget-stations') + '&t=' + new Date().getTime();

                $.get(url).done(function(html)
                {
                    $(html +
                      '<div class="dashboard-action" style="display:none;position:absolute;top:0px;width:' + el.width() + 'px;">' +
                      '<a class="dashboard-action" style="float:right;"><i class="fa fa-times-circle" aria-hidden="true"></i></a>' +
                      '<a class="dashboard-action" style="float:right; padding-right:5px;"><i class="fa fa-edit" aria-hidden="true"></i></a><span style="clear:right;"></span></div>')
                    .appendTo(el);

                    el.unbind('mouseenter').unbind('mouseleave');

                    el.bind('mouseenter', () => { el.find('div.dashboard-action').show() });
                    el.bind('mouseleave', () => { el.find('div.dashboard-action').hide() });
                });
            }
        });
    }

    var widgetToHtml = function (widget, stations)
    {
        return '<li style="background-color:' + widget.background + ';" data-widget-stations="' + stations.join(',') + '" data-widget-guid="' + widget.guid + '" data-widget-url="' + widget.url + '" data-widget-last-sync="0"></li>';
    }

    this.addWidget = function (widget, stations, position)
    {
        gridster.add_widget(widgetToHtml(widget, stations), widget.width, widget.height);
        this.refresh();
        storeWidgets();
    }

    setInterval(this.refresh, 1000 * 15);
};