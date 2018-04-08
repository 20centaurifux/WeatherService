var Dashboard = function(opts)
{
    var options = opts || {};
    var self = this;

    var storeWidgets = function()
    {
        if (options.onSave)
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

            options.onSave(items);
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
                setTimeout(storeWidgets, 0);
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

            if (!lastSync || now - lastSync >= 120)
            {
                el.html('<img src="/assets/rings.svg" style="position:absolute; margin:auto; top:0; left:0; right:0; bottom:0;">');

                setTimeout(() =>
                {
                    el.attr('data-widget-last-sync', now);

                    var url = el.attr('data-widget-url') + '?s=' + el.attr('data-widget-stations') + '&t=' + new Date().getTime();

                    $.get(url).done(function(html)
                    {
                        el.html('');

                        var content = $(html +
                          '<div class="dashboard-action"><h1 class="dashboard-action">Edit</h1>' +
                          '<a class="dashboard-action" style="float:right;"><i class="fa fa-times-circle dashboard-action" aria-hidden="true"></i></a>' +
                          '<a class="dashboard-action" style="float:right; padding-right:5px;"><i class="fa fa-edit dashboard-action" aria-hidden="true"></i></a><span style="clear:right;"></span></div>')
                        .appendTo(el);

                        el.unbind('mouseenter').unbind('mouseleave');

                        el.bind('mouseenter', () => { el.find('div.dashboard-action').show() });
                        el.bind('mouseleave', () => { el.find('div.dashboard-action').hide() });

                        content.find('a:eq(0)').bind('click', () =>
                        {
                            gridster.remove_widget(el.get(0), storeWidgets);
                        });

                        if (options.onEdit)
                        {
                            content.find('a:eq(1)').bind('click', () =>
                            {
                                options.onEdit(el.get(0), el.attr('data-widget-guid'), el.attr('data-widget-stations').split(','));
                            });
                        }
                    });
                }, Math.floor(Math.random() * 1800) + 500);
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

    this.updateWidget = function (el, stations)
    {
        $(el).attr('data-widget-stations', stations.join(',')).attr('data-widget-last-sync', '0');
        this.refresh();
        storeWidgets();
    }

    setInterval(this.refresh, 1000 * 15);
};