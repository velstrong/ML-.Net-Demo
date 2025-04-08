/* eslint-disable react/jsx-wrap-multilines */
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Button, message, PageHeader } from 'antd';
import mjml from 'mjml-browser';
import {
  EmailEditor,
  EmailEditorProvider,
  IEmailTemplate,
  Stack,
  useEditorContext,
} from 'easy-email-editor';
import 'easy-email-editor/lib/style.css';
import 'antd/dist/antd.css';

import { useImportTemplate } from '../../hooks/useImportTemplate';
import { useExportTemplate } from '../../hooks/useExportTemplate';
import { copy } from '../../utils/clipboard';
import { AdvancedType, BasicType, BlockManager, JsonToMjml } from 'easy-email-core';
import { ExtensionProps, MjmlToJson, StandardLayout } from 'easy-email-extensions';
import { FormApi } from 'final-form';
import 'easy-email-editor/lib/style.css';
import 'easy-email-extensions/lib/style.css';
import '@arco-themes/react-easy-email-theme-purple/css/arco.css';
import {useWindowSize} from 'react-use'
import { data, useNavigate, useParams } from "react-router-dom";
import { dataList } from "../../constant/data";
import '../../CustomBlocks';
import { CustomBlocksType } from '../../CustomBlocks/constants';
import { saveAs } from 'file-saver';
import axios from "axios";
import {
  Input,
  Modal,
} from "@arco-design/web-react";
import ChatGPT from '../../components/ChatGPT';

const fontList = [
  'Arial',
  'Tahoma',
  'Verdana',
  'Times New Roman',
  'Courier New',
  'Georgia',
  'Lato',
  'Montserrat'
].map(item => ({ value: item, label: item }));

const categories: ExtensionProps['categories'] = [
  {
    label: 'Content',
    active: true,
    blocks: [
      {
        type: AdvancedType.TEXT,
      },
      {
        type: AdvancedType.IMAGE,
        payload: { attributes: { padding: '0px 0px 0px 0px' } },
      },
      {
        type: AdvancedType.BUTTON,
      },
      {
        type: AdvancedType.SOCIAL,
      },
      {
        type: AdvancedType.DIVIDER,
      },
      {
        type: AdvancedType.SPACER,
      },
      {
        type: AdvancedType.HERO,
      },
      {
        type: AdvancedType.WRAPPER,
      },
      {
        type: CustomBlocksType.MY_FIRST_BLOCK,
      },
      {
        type: CustomBlocksType.ENBD_SOCIAL,
      },
      {
        type: CustomBlocksType.IMAGE_WITH_TEXT,
      },
      {
        type: CustomBlocksType.PRODUCT_RECOMMENDATION,
      },
    ],
  },
  {
    label: 'Layout',
    active: true,
    displayType: 'column',
    blocks: [
      {
        title: '2 columns',
        payload: [
          ['50%', '50%'],
          ['33%', '67%'],
          ['67%', '33%'],
          ['25%', '75%'],
          ['75%', '25%'],
          ['17%', '83%'],
          ['11%', '89%'],
        ],
      },
      {
        title: '3 columns',
        payload: [
          ['33.33%', '33.33%', '33.33%'],
          ['25%', '25%', '50%'],
          ['50%', '25%', '25%'],
        ],
      },
      {
        title: '4 columns',
        payload: [[['25%', '25%', '25%', '25%']]],
      },
    ],
  },
];

const pageBlock = BlockManager.getBlockByType(BasicType.PAGE)!;


export default function Editor() {
  const [downloadFileName, setDownloadName] = useState('download.mjml');
  const [fileLoad, setfileLoad] = useState(false);
  const [collapsed, setCollapsed] = React.useState(false);
    
  const [template, setTemplate] = useState<IEmailTemplate['content']>(pageBlock.create({
    data: {
      value: {
        "content-background-color": '#ffffff'
      }
    }
  }));
  
  const { id } = useParams();

  const appData = useMemo(() => {
    if (id == "-1") return undefined;
    return dataList.find((i) => i.id === Number(id))?.tree;
  }, [id]);
 
  //const [template, setTemplate] = useState<IEmailTemplate['content']>();
  const { importTemplate } = useImportTemplate();
  const { exportTemplate } = useExportTemplate();
 
  const { width } = useWindowSize();

  const smallScene = width < 1400;
  const fetchData = async () => {
    try {
      const response = await axios.get(`../../src/templates/${id}.mjml`).then((res) => {
        setTemplate(MjmlToJson(res.data as any));
        setfileLoad(true);
      });
    } catch (error) {
      console.error(error);
    }
  };
  if(!fileLoad)
  {
  fetchData();
  }
  const onCopyHtml = (values: IEmailTemplate) => {
    const html = mjml(JsonToMjml({
      data: values.content,
      mode: 'production',
      context: values.content
    }), {
      beautify: true,
      validationLevel: 'soft',
    }).html;

    copy(html);
    message.success('Copied to pasteboard!')
  };
  const openChatGPT = () => {
   setCollapsed(true);
  };
  const onImportMjml = async () => {
    try {
      const [filename, data] = await importTemplate();
      setDownloadName(filename);
      setTemplate(data);
    } catch (error) {
      message.error('Invalid mjml file');
    }
  };


  const onExportMjml = (values: IEmailTemplate) => {
    exportTemplate(
      downloadFileName,
      JsonToMjml({
        data: values.content,
        mode: 'production',
        context: values.content
      }))
  };

  
  const onSubmit = useCallback(
    async (
      values: IEmailTemplate,
      form: FormApi<IEmailTemplate, Partial<IEmailTemplate>>
    ) => {
      const mjmldata= JsonToMjml({
        data: values.content,
        mode: 'production',
        context: values.content
      });
      var blob = new Blob([mjmldata]);
     await saveAs(blob, `${id}.mjml`);
     message.success('Saved success!')
     //form.restart(mjmldata); //replace new values form backend 
    },
    []
  );

  const onUploadImage = async (blob: Blob) => {
    console.log(blob);
    return 'https://yourimageshare.com/ib/K4vJkuZtiP.png';
  };

const navigate = useNavigate();

  const initialValues: IEmailTemplate | null = useMemo(() => {
    return {
      subject: 'Welcome to Easy-email',
      subTitle: 'Nice to meet you!',
      content: template
    };
  }, [template]);


  if (!initialValues) return null;

  return (
    <>
    <div>
      <EmailEditorProvider
        dashed={false}
        data={initialValues}
        height={'calc(100vh - 85px)'}
        onUploadImage={onUploadImage}
        autoComplete
        fontList={fontList}
        onSubmit={onSubmit}
      >
        {({ values }, { submit }) => {
          return (
            <>
              <PageHeader
                title='Edit'
                extra={
                  <Stack alignment="center">
                    <Button onClick={() => openChatGPT()}>
                      AI
                    </Button>
                    <Button onClick={() =>  navigate("/home")}>
                      Dashboard
                    </Button>
                    <Button onClick={() => onCopyHtml(values)}>
                      Copy Html
                    </Button>
                    <Button onClick={() => onExportMjml(values)}>
                      Export Template
                    </Button>
                    <Button onClick={onImportMjml}>
                      import Template
                    </Button>
                    <Button
                      type='primary'
                      onClick={() => submit()}
                    >
                      Save
                    </Button>
                  </Stack>
                }
              />

          <StandardLayout
            compact={!smallScene}
            categories={categories}
            showSourceCode={true}
          >
            <EmailEditor />
          </StandardLayout>
            </>
          );
        }}
      </EmailEditorProvider>
    </div>
   
   {collapsed && <ChatGPT isvisible={collapsed}/>}
    
    </>
  );
}
